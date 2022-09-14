namespace PromoterBot.Models
{
    public class Participant
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string City { get; set; }

        public string Gender { get; set; }

        public int Age { get; set; }

        public string FavouriteBrands { get; set; }

        public string SocialNetwork { get; set; }

        public string Images { get; set; }

        public int PromoterId { get; set; }

        public Promoter Promoter { get; set; }
    }
}
