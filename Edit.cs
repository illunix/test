using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ravency.Core.Entities;
using Ravency.Infrastructure.Data;
using Ravency.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ravency.Web.Areas.Configuration.Languages
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public Guid Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m => m.Id).NotNull();
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _context;
            private readonly IConfigurationProvider _configuration;

            public QueryHandler(ApplicationDbContext context, IConfigurationProvider configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<Command> Handle(Query request, CancellationToken cancellationToken)
            {
                var languages = await _context.Languages
                    .ProjectTo<Command.Language>(_configuration)
                    .ToListAsync();

                var command = await _context.Languages
                    .Where(language => language.Id == request.Id)
                    .ProjectTo<Command>(_configuration)
                    .SingleOrDefaultAsync();

                command.Languages = languages;

                return command;
            }
        }

        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public bool IsActive { get; set; }
            public bool IsDefault { get; set; }
            public List<Language> Languages { get; set; }

            public class Language
            {
                public Guid Id { get; set; }
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
                var language = await _context.Languages
                    .FindAsync(request.Id);

                _mapper.Map(request, language);

                if (request.IsDefault)
                {
                    language = await _context.Languages
                        .Where(language => language.IsDefault)
                        .SingleOrDefaultAsync();

                    language.IsDefault = false;
                }

                _context.Update(language);

                await _context.SaveChangesAsync();
            }
        }
    }
}
