using AutoMapper;
using Ravency.Application.ProductCategories.Commands.AddProductCategory;
using Ravency.Application.ProductCategories.DTO;
using Ravency.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ravency.Application.ProductCategories
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddProductCategoryCommand, ProductCategoryLocale>();
            CreateMap<ProductCategoryDto, ProductCategory>();
            CreateMap<AddProductCategoryCommand, ProductCategoryLocale>()
                .ForMember(d => d.CategoryId, opt => opt.MapFrom(c => c.Language.ProductCategory.Id))
                .ForMember(d => d.LanguageId, opt => opt.MapFrom(c => c.Language.Id))
                .ForMember(d => d.LanguageId, opt => opt.MapFrom(c => c.Language.ProductCategory.Name));
        }
    }
}
