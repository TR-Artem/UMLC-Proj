public interface ITradingService
{
    Task<Transaction> BuySkin(int buyerId, int skinId);
    Task<Skin> ListSkinForSale(int userId, int skinId, decimal price);
    Task CancelSale(int skinId);
    Task<IEnumerable<Skin>> GetAvailableSkins();
    Task<IEnumerable<Skin>> GetUserInventory(int userId);
}

public class TradingService : ITradingService
{
    private readonly IUserRepository _userRepository;
    private readonly ISkinRepository _skinRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ITransactionRepository _transactionRepository;
    
    public async Task<Transaction> BuySkin(int buyerId, int skinId)
    {
        using var transaction = await _transactionRepository.BeginTransactionAsync();
        
        try
        {
            var buyer = await _userRepository.GetByIdAsync(buyerId);
            var skin = await _skinRepository.GetByIdAsync(skinId);
            var seller = await _userRepository.GetByIdAsync(skin.InventoryItems.First().UserId);
            
            if (buyer.Balance < skin.Price)
                throw new InsufficientFundsException("Недостаточно средств для покупки");
            
            // Расчет комиссии (10%)
            var commission = skin.Price * 0.1m;
            var sellerAmount = skin.Price - commission;
            
            // Обновление балансов
            buyer.Balance -= skin.Price;
            seller.Balance += sellerAmount;
            
            // Создание транзакции
            var newTransaction = new Transaction
            {
                BuyerId = buyerId,
                SellerId = seller.Id,
                SkinId = skinId,
                Amount = skin.Price,
                Commission = commission,
                TransactionDate = DateTime.UtcNow,
                Status = "Completed"
            };
            
            // Перенос скина
            var inventoryItem = await _inventoryRepository.GetByUserAndSkinAsync(seller.Id, skinId);
            inventoryItem.UserId = buyerId;
            inventoryItem.AcquiredDate = DateTime.UtcNow;
            
            // Сохранение изменений
            await _userRepository.UpdateAsync(buyer);
            await _userRepository.UpdateAsync(seller);
            await _inventoryRepository.UpdateAsync(inventoryItem);
            await _transactionRepository.AddAsync(newTransaction);
            
            await transaction.CommitAsync();
            return newTransaction;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    // Другие методы...
}
