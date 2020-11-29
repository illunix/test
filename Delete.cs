using FluentValidation;
using MediatR;
using Ravency.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ravency.Web.Areas.Configuration.Languages
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(command => command.Id)
                    .NotNull();
            }
        }

        public class CommandHandler : AsyncRequestHandler<Command>
        {
            private readonly ApplicationDbContext _context;

            public CommandHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            protected override async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var language = await _context.Languages
                    .FindAsync(request.Id);

                _context.Languages
                    .Remove(language);

                await _context.SaveChangesAsync();
            }
        }
    }
}
