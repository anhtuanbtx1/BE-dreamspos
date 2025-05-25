namespace PosStore.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? Unit { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? Unit { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImagePath { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? Unit { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImagePath { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class ProductStockDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsLowStock { get; set; }
    }
}
