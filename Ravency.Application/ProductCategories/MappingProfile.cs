using AutoMapper;
using Ravency.Application.ProductCategories.Commands;
using Ravency.Application.Shared.DTO;
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
            CreateMap<LanguageDto<ProductCategory>, Language>();
            CreateMap<Language, LanguageDto<ProductCategory>>();

            CreateMap<AddProductCategory.Command, ProductCategory>();

            CreateMap<ProductCategory, ProductCategoryLocale>()
                .ForMember(d => d.CategoryId, opt => opt.MapFrom(c => c.Id))
                .ForMember(d => d.Name, opt => opt.Ignore());

            CreateMap<LanguageDto<ProductCategory>, ProductCategory>()
                .ForMember(d => d.Id, opt => opt.MapFrom(c => c.Data.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(c => c.Data.Name));

            CreateMap<LanguageDto<ProductCategory>, ProductCategoryLocale>()
                .ForMember(d => d.LanguageId, opt => opt.MapFrom(c => c.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(c => c.Data.Name));
        }
    }
}
