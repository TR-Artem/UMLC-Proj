// ITransactionService.cs
public interface ITransactionService
{
    Task<Transaction> CreateTransactionAsync(int buyerId, int sellerId, int skinId, decimal amount);
    Task CompleteTransactionAsync(int transactionId);
    Task CancelTransactionAsync(int transactionId);
    Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId);
}

// TransactionService.cs
public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TransactionService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IInventoryService _inventoryService;

    public TransactionService(
        AppDbContext context,
        ILogger<TransactionService> logger,
        IUserRepository userRepository,
        IInventoryService inventoryService)
    {
        _context = context;
        _logger = logger;
        _userRepository = userRepository;
        _inventoryService = inventoryService;
    }

    public async Task<Transaction> CreateTransactionAsync(int buyerId, int sellerId, int skinId, decimal amount)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Проверяем, что покупатель и продавец существуют
            var buyerExists = await _userRepository.ExistsAsync(buyerId);
            var sellerExists = await _userRepository.ExistsAsync(sellerId);

            if (!buyerExists || !sellerExists)
            {
                throw new ArgumentException("Buyer or seller does not exist");
            }

            // Проверяем, что у продавца есть этот скин
            var hasSkin = await _inventoryService.HasSkinAsync(sellerId, skinId);
            if (!hasSkin)
            {
                throw new InvalidOperationException("Seller does not have this skin");
            }

            // Проверяем баланс покупателя
            var buyer = await _userRepository.GetByIdAsync(buyerId);
            if (buyer.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds");
            }

            // Создаем транзакцию
            var newTransaction = new Transaction
            {
                BuyerId = buyerId,
                SellerId = sellerId,
                SkinId = skinId,
                Amount = amount,
                Status = "Pending"
            };

            await _context.Transactions.AddAsync(newTransaction);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return newTransaction;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating transaction. Buyer: {BuyerId}, Seller: {SellerId}, Skin: {SkinId}", 
                buyerId, sellerId, skinId);
            throw;
        }
    }

    public async Task CompleteTransactionAsync(int transactionId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var dbTransaction = await _context.Transactions.FindAsync(transactionId);
            if (dbTransaction == null)
            {
                throw new KeyNotFoundException("Transaction not found");
            }

            if (dbTransaction.Status != "Pending")
            {
                throw new InvalidOperationException("Transaction is not in pending state");
            }

            // Получаем участников сделки
            var buyer = await _userRepository.GetByIdAsync(dbTransaction.BuyerId);
            var seller = await _userRepository.GetByIdAsync(dbTransaction.SellerId);

            // Проверяем баланс покупателя
            if (buyer.Balance < dbTransaction.Amount)
            {
                throw new InvalidOperationException("Buyer has insufficient funds");
            }

            // Проверяем, что скин еще у продавца
            var hasSkin = await _inventoryService.HasSkinAsync(seller.Id, dbTransaction.SkinId);
            if (!hasSkin)
            {
                throw new InvalidOperationException("Seller no longer has this skin");
            }

            // Переводим деньги
            buyer.Balance -= dbTransaction.Amount;
            seller.Balance += dbTransaction.Amount;

            // Передаем скин
            await _inventoryService.RemoveSkinFromInventoryAsync(seller.Id, dbTransaction.SkinId);
            await _inventoryService.AddSkinToInventoryAsync(buyer.Id, dbTransaction.SkinId);

            // Обновляем статус транзакции
            dbTransaction.Status = "Completed";
            dbTransaction.TransactionDate = DateTime.UtcNow;

            await _userRepository.UpdateAsync(buyer);
            await _userRepository.UpdateAsync(seller);
            _context.Transactions.Update(dbTransaction);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error completing transaction ID: {TransactionId}", transactionId);
            throw;
        }
    }

    public async Task CancelTransactionAsync(int transactionId)
    {
        try
        {
            var dbTransaction = await _context.Transactions.FindAsync(transactionId);
            if (dbTransaction == null)
            {
                throw new KeyNotFoundException("Transaction not found");
            }

            if (dbTransaction.Status != "Pending")
            {
                throw new InvalidOperationException("Only pending transactions can be cancelled");
            }

            dbTransaction.Status = "Cancelled";
            _context.Transactions.Update(dbTransaction);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling transaction ID: {TransactionId}", transactionId);
            throw;
        }
    }

    public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId)
    {
        try
        {
            return await _context.Transactions
                .Where(t => t.BuyerId == userId || t.SellerId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transactions for user ID: {UserId}", userId);
            throw;
        }
    }
}