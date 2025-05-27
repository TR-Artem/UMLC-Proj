// IInventoryService.cs
public interface IInventoryService
{
    Task<IEnumerable<Skin>> GetUserInventoryAsync(int userId);
    Task AddSkinToInventoryAsync(int userId, int skinId);
    Task RemoveSkinFromInventoryAsync(int userId, int skinId);
    Task<bool> HasSkinAsync(int userId, int skinId);
}

// InventoryService.cs
public class InventoryService : IInventoryService
{
    private readonly AppDbContext _context;
    private readonly ILogger<InventoryService> _logger;
    private readonly ISteamIntegrationService _steamIntegration;

    public InventoryService(
        AppDbContext context, 
        ILogger<InventoryService> logger,
        ISteamIntegrationService steamIntegration)
    {
        _context = context;
        _logger = logger;
        _steamIntegration = steamIntegration;
    }

    public async Task<IEnumerable<Skin>> GetUserInventoryAsync(int userId)
    {
        try
        {
            return await _context.InventoryItems
                .Where(i => i.UserId == userId)
                .Join(_context.Skins,
                    inventory => inventory.SkinId,
                    skin => skin.Id,
                    (inventory, skin) => skin)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory for user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task AddSkinToInventoryAsync(int userId, int skinId)
    {
        try
        {
            // Проверяем, есть ли уже такой скин у пользователя
            if (await _context.InventoryItems.AnyAsync(i => i.UserId == userId && i.SkinId == skinId))
            {
                throw new InvalidOperationException("User already has this skin in inventory");
            }

            var inventoryItem = new InventoryItem
            {
                UserId = userId,
                SkinId = skinId
            };

            await _context.InventoryItems.AddAsync(inventoryItem);
            await _context.SaveChangesAsync();

            // Синхронизируем с Steam
            var skin = await _context.Skins.FindAsync(skinId);
            await _steamIntegration.TransferSkinAsync(skin.SteamItemId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding skin to inventory. User ID: {UserId}, Skin ID: {SkinId}", userId, skinId);
            throw;
        }
    }

    public async Task RemoveSkinFromInventoryAsync(int userId, int skinId)
    {
        try
        {
            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.UserId == userId && i.SkinId == skinId);

            if (inventoryItem == null)
            {
                throw new KeyNotFoundException("Skin not found in user's inventory");
            }

            _context.InventoryItems.Remove(inventoryItem);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing skin from inventory. User ID: {UserId}, Skin ID: {SkinId}", userId, skinId);
            throw;
        }
    }

    public async Task<bool> HasSkinAsync(int userId, int skinId)
    {
        try
        {
            return await _context.InventoryItems
                .AnyAsync(i => i.UserId == userId && i.SkinId == skinId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user has skin. User ID: {UserId}, Skin ID: {SkinId}", userId, skinId);
            throw;
        }
    }
}