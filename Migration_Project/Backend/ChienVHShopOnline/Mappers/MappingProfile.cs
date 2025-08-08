using AutoMapper;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.DTOs;
using ChienVHShopOnline.Models.Enums;

namespace ChienVHShopOnline.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateMap<User, UserResponseDto>().ReverseMap();
            // CreateMap<UserRequestDto, User>();
            // CreateMap<UserUpdateDto, User>();

            CreateMap<UserRegisterRequestDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) 
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim().ToLowerInvariant()))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => UserRole.User));

            CreateMap<User, UserLoginResponseDto>()
                .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());

            CreateMap<UserRequestDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<User, UserResponseDto>();

            CreateMap<UserUpdateDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ProductAddDto, Product>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());

            CreateMap<ProductUpdateDto, Product>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());

            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color.Name))
                .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.Model.Name))
                .ForMember(dest => dest.StorageName, opt => opt.MapFrom(src => src.Storage.Name))
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src => src.IsNew == 1))
                .ForMember(dest => dest.SellStartDate,
                    opt => opt.MapFrom(src =>
                        src.SellStartDate.HasValue
                            ? TimeZoneInfo.ConvertTimeFromUtc(src.SellStartDate.Value, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).ToString("yyyy-MM-dd")
                            : null
                    )
                )
                .ForMember(dest => dest.SellEndDate,
                    opt => opt.MapFrom(src =>
                        src.SellEndDate.HasValue
                            ? TimeZoneInfo.ConvertTimeFromUtc(src.SellEndDate.Value, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).ToString("yyyy-MM-dd")
                            : null
                    )
                );

            CreateMap<Color, ColorResponseDto>().ReverseMap();
            CreateMap<ColorRequestDto, Color>();

            CreateMap<Category, CategoryResponseDto>().ReverseMap();
            CreateMap<CategoryRequestDto, Category>();

            CreateMap<Storage, StorageResponseDto>().ReverseMap();
            CreateMap<StorageRequestDto, Storage>();

            CreateMap<Model, ModelResponseDto>().ReverseMap();
            CreateMap<ModelRequestDto, Model>();

            CreateMap<Order, OrderResponseDto>().ReverseMap();
            CreateMap<OrderRequestDto, Order>()
                .ForMember(dest => dest.OrderDate, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<OrderDetail, OrderDetailsResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName));
            CreateMap<OrderDetailRequestDto, OrderDetail>();

            CreateMap<News, NewsResponseDto>().ReverseMap();
            CreateMap<NewsRequestDto, News>()
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());

            CreateMap<ContactUsRequestDto, ContactUs>();
            CreateMap<ContactUs, ContactUsResponseDto>();

        }
    }
}
