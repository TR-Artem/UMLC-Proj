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
