
namespace ShopApi.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get { return Quantity * Price; } }
        public string ImageName { get; set; }
        public int RegisterUserId { get; set; }
        public virtual RegisterUser RegisterUser { get; set; }

        public CartItem()
        {

        }
    }
}