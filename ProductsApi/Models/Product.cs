namespace ProductsApi.Models
{
    // This class represents a product in our store
    public class Product
    {
        // Each product needs a unique ID to identify it
        public int Id { get; set; }

        // The name of our product
        public string Name { get; set; }

        // How much the product costs
        public decimal Price { get; set; }
    }
}
