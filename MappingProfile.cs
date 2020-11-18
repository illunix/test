using AutoMapper;
using Ravency.Core.Entities;
using Ravency.Web.Areas.Catalog.ProductCategories;
using Ravency.Web.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ravency.Web.Areas.Catalog.ProductCategories
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductCategory, Index.Result.ProductCategory>();

            CreateMap<Language<ProductCategory>, Language>();
            CreateMap<Language, Language<ProductCategory>>();

            CreateMap<Add.Command, ProductCategory>()
                .ForMember(d => d.Id, opt => opt.Ignore());

            CreateMap<Language<ProductCategory>, ProductCategory>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Name, opt => opt.MapFrom(c => c.Data.Name))
                .ForMember(d => d.Gender, opt => opt.MapFrom(c => c.Data.Gender));

            CreateMap<Language<ProductCategory>, ProductCategoryLocale>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CategoryId, opt => opt.MapFrom(c => c.Data.Id))
                .ForMember(d => d.LanguageId, opt => opt.MapFrom(c => c.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(c => c.Data.Name));
        }
    }
}
