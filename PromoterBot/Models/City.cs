namespace PromoterBot.Models
{
    public class City : Location
    {
        public int RegionId { get; set; }

        public Region Region { get; set; }
    }
}
