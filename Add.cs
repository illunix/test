using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ravency.Core.Entities;
using Ravency.Infrastructure.Data;
using Ravency.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ravency.Web.Areas.Configuration.Languages
{
    public class Add
    {
        public class Query : IRequest<Command>
        {
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            public async Task<Command> Handle(Query query, CancellationToken cancellationToken)
            {
                var languages = await GetLanguages();

                return new Command
                {
                    Languages = languages
                };
            }
        }

        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public bool IsActive { get; set; }
            public bool IsDefault { get; set; }
            public List<SelectListItem> Languages { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(ApplicationDbContext context)
            {
                RuleFor(language => language.Name)
                    .NotEmpty().WithMessage("Please select language")
                    .MustAsync(async (name, cancellationToken) =>
                    {
                        var exist = await context.Languages
                            .AnyAsync(x => x.Name == name);

                        return !exist;
                    }).WithMessage("This language already has been added");
            }
        }

        public class CommandHandler : AsyncRequestHandler<Command>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public CommandHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            protected override async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var language = _mapper.Map<Command, Language>(request);

                var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();

                var culture = cultures
                    .Where(culture => culture.EnglishName == request.Name)
                    .SingleOrDefault();

                _mapper.Map(culture, language);

                _context.Languages
                    .Add(language);

                if (request.IsDefault)
                {
                    language = await _context.Languages
                        .Where(language => language.IsDefault)
                        .SingleOrDefaultAsync();

                    language.IsDefault = false;

                    _context.Update(language);
                }

                await _context.SaveChangesAsync();
            }
        }

        public class CommandPostProcessor : IRequestPostProcessor<Command, Unit>
        {
            public async Task Process(Command command, Unit response, CancellationToken cancellationToken)
            {
                var languages = await GetLanguages();

                command.Languages = languages;
            }
        }

        public static async Task<List<SelectListItem>> GetLanguages()
        {
            var languages = new List<SelectListItem>();

            await Task.Run(() =>
            {
                var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures).OrderBy(x => x.Name).ToList();
                foreach (var culture in cultures)
                {
                    var language = new SelectListItem { Value = culture.EnglishName, Text = culture.EnglishName };

                    languages.Add(language);
                }

                languages.RemoveAt(0);
            });

            return languages;
        }
    }
}
