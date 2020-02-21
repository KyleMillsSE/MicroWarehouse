namespace MicroPicking.Domain
{
    public class Pallet
    {
        public int Id { get; set; }
        public string PalletNumber { get; set; }
        public string LineDescription { get; set; }
        public int LineQuantity { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
