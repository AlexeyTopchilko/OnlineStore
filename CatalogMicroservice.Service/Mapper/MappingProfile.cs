using AutoMapper;
using CatalogMicroservice.Domain.Entities;
using CatalogMicroservice.Repository.Models.ResponseModels;
using CatalogMicroservice.Service.Services.CategoryService.Models;
using CatalogMicroservice.Service.Services.ProductService.Models;

namespace CatalogMicroservice.Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductsCategories, CategoryDto>()
                .ForMember(_ => _.CategoryName, _ => _.MapFrom(_ => _.Category.Name));

            CreateMap<CategoryDto, ProductsCategories>()
                .ForMember(_ => _.CategoryId, _ => _.MapFrom(_ => _.CategoryId));

            CreateMap<Product, ProductView>()
                .ForMember(_ => _.Categories, _ => _.MapFrom(_ => _.ProductsCategories))
                .ReverseMap();

            CreateMap<Category, CategoryView>()
                .ForMember(_ => _.Products, _ => _.MapFrom(_ => _.ProductsCategories))
                .ReverseMap();

            CreateMap<ProductsCategories, ProductDto>()
                .ForMember(_ => _.ProductName, _ => _.MapFrom(_ => _.Product.Name))
                .ReverseMap();

            CreateMap<ProductsResponse<Product>, ProductsView>()
                .ReverseMap();
        }
    }
}