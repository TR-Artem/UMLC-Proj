public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<InventoryItem> InventoryItems { get; set; }
    public ICollection<Transaction> BuyerTransactions { get; set; }
    public ICollection<Transaction> SellerTransactions { get; set; }
}

public class Skin
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Game { get; set; }
    public string Rarity { get; set; }
    public decimal Price { get; set; }
    public string Condition { get; set; }
    public string SteamItemId { get; set; }
    public ICollection<InventoryItem> InventoryItems { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
}

public class InventoryItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int SkinId { get; set; }
    public Skin Skin { get; set; }
    public DateTime AcquiredDate { get; set; }
}

public class Transaction
{
    public int Id { get; set; }
    public int BuyerId { get; set; }
    public User Buyer { get; set; }
    public int? SellerId { get; set; }
    public User Seller { get; set; }
    public int SkinId { get; set; }
    public Skin Skin { get; set; }
    public decimal Amount { get; set; }
    public decimal Commission { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Status { get; set; }
}
