public class InventoryItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int SkinId { get; set; }
    public Skin Skin { get; set; }
    public DateTime AcquiredDate { get; set; }
}
