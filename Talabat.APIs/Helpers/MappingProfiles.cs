using AutoMapper;
using Microsoft.Extensions.Configuration;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Order_Aggregate;

//using Talabat.Core.Entities.Identity;

//using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Product_Aggregate;

namespace Talabat.APIs.Helpers
{
    public class MappingProfiles : Profile
    {


        public MappingProfiles()
        {


            CreateMap<Product, ProductToReturnDto>()
                .ForMember(P=>P.Brand, O=>O.MapFrom(S=>S.Brand.Name))
                .ForMember(P => P.Category, O => O.MapFrom(S => S.Category.Name))
                .ForMember(P=>P.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());

            CreateMap<BasketItemDto, BasketItem>();
            CreateMap<CustomerBasketDto, CustomerBasket>();
            CreateMap<Address, AddressDto>().ReverseMap();
            //CreateMap<Talabat.Core.Entities.Identity.Address, AddressDto>().ReverseMap();


            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod, O => O.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.DeliveryMethodCost, O => O.MapFrom(s => s.DeliveryMethod.Cost));


            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, O => O.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.ProductName, O => O.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.PictureUrl, O => O.MapFrom(s => s.Product.PictureUrl))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());


        }
    }
}
