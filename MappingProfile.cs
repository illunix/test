using AutoMapper;
using Ravency.Core.Entities;
using Ravency.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Ravency.Web.Areas.Configuration.Languages
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Add.Command, Language>();

            CreateMap<Edit.Command, Language>();

            CreateMap<Language, Edit.Command.Language>();

            CreateMap<Language, Edit.Command>();

            CreateMap<Edit.Command.Language, Edit.Command>();

            CreateMap<CultureInfo, Language>()
                .ForMember(d => d.Name, opt => opt.MapFrom(c => c.EnglishName))
                .ForMember(d => d.Code, opt => opt.MapFrom(c => c.Name));
        }
    }
}
