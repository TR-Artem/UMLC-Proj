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
