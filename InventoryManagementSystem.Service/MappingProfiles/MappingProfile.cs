using AutoMapper;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Service.DTOs;

namespace InventoryManagementSystem.Service.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : null));
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();

            // InventoryTransaction mappings
            CreateMap<InventoryTransaction, InventoryTransactionDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.SourceWarehouseName, opt => opt.MapFrom(src => src.SourceWarehouse != null ? src.SourceWarehouse.Name : null))
                .ForMember(dest => dest.DestinationWarehouseName, opt => opt.MapFrom(src => src.DestinationWarehouse != null ? src.DestinationWarehouse.Name : null))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : null));
            CreateMap<InventoryTransactionCreateDto, InventoryTransaction>();
            
            // Warehouse mappings
            CreateMap<Warehouse, WarehouseDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
            CreateMap<WarehouseCreateDto, Warehouse>();
            CreateMap<WarehouseUpdateDto, Warehouse>();

            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();
            
            // Notification mappings
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : null));
            
            // TransactionArchive mappings
            CreateMap<TransactionArchive, InventoryTransactionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OriginalTransactionId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.SourceWarehouseName, opt => opt.MapFrom(src => src.SourceWarehouseName))
                .ForMember(dest => dest.DestinationWarehouseName, opt => opt.MapFrom(src => src.DestinationWarehouseName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));
        }
    }
}
