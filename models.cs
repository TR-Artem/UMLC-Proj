// User.cs
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// Skin.cs
public class Skin
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Game { get; set; }
    public string Rarity { get; set; }
    public decimal Price { get; set; }
    public string Condition { get; set; }
    public string SteamItemId { get; set; }
}

// InventoryItem.cs
public class InventoryItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SkinId { get; set; }
    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
}

// Transaction.cs
public class Transaction
{
    public int Id { get; set; }
    public int BuyerId { get; set; }
    public int SellerId { get; set; }
    public int SkinId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
}