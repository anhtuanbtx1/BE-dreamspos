using AutoMapper;
using PosStore.DTOs;
using PosStore.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PosStore.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product mappings
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SKU, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            // Customer mappings
            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.TotalPurchases, opt => opt.MapFrom(src => src.Sales.Count))
                .ForMember(dest => dest.TotalSpent, opt => opt.MapFrom(src => src.Sales.Sum(s => s.TotalAmount)));
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());

            // Sale mappings
            CreateMap<Sale, SaleDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : null));
            CreateMap<CreateSaleDto, Sale>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SaleDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Completed"));

            // SaleItem mappings
            CreateMap<SaleItem, SaleItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.Product.SKU));
            CreateMap<CreateSaleItemDto, SaleItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SaleId, opt => opt.Ignore());
        }
    }
}
