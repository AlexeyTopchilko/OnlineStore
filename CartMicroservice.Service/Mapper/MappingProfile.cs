using AutoMapper;
using CartMicroservice.Domain.Entities;
using CartMicroservice.Service.Services.CartService.Models;
using CartMicroservice.Service.Services.RabbitMqService.Models;

namespace CartMicroservice.Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CartProducts, ProductsRequestModel>();

            CreateMap<Domain.Entities.Cart, CartViewModel>();
        }
    }
}