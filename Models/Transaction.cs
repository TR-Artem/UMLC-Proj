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
