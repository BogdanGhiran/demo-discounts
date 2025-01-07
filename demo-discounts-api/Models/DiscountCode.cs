namespace demo_discounts_api.Models
{
    public class DiscountCode
    {
        public string Code { get; set; }
        public bool IsUsed { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUsed { get; set; }
    }
}
