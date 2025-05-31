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

            // Project mappings
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser != null ? $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}" : null));
            CreateMap<Project, ProjectSummaryDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));
            CreateMap<CreateProjectDto, Project>();
            CreateMap<UpdateProjectDto, Project>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
                // .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());  // Temporarily disabled

            // WeddingGuest mappings
            CreateMap<WeddingGuest, WeddingGuestDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Relationship, opt => opt.MapFrom(src => src.Relationship.HasValue ? src.Relationship.Value.ToString() : null));
            CreateMap<WeddingGuest, WeddingGuestSummaryDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Relationship, opt => opt.MapFrom(src => src.Relationship.HasValue ? src.Relationship.Value.ToString() : null));
            CreateMap<CreateWeddingGuestDto, WeddingGuest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<GuestStatus>(src.Status, true)))
                .ForMember(dest => dest.Relationship, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Relationship) ? Enum.Parse<RelationshipType>(src.Relationship, true) : (RelationshipType?)null))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());
            CreateMap<UpdateWeddingGuestDto, WeddingGuest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<GuestStatus>(src.Status, true)))
                .ForMember(dest => dest.Relationship, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Relationship) ? Enum.Parse<RelationshipType>(src.Relationship, true) : (RelationshipType?)null))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());
        }
    }
}
