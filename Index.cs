using MediatR;
using Microsoft.EntityFrameworkCore;
using Ravency.Core.Entities;
using Ravency.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ravency.Web.Areas.Configuration.Languages
{
    public class Index
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public Guid Id { get; set; }
            public List<Language> Languages { get; set; }
        }

        private class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly ApplicationDbContext _context;

            public QueryHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                var languages = await _context.Languages
                    .ToListAsync();

                return new Result
                {
                    Languages = languages
                };
            }
        }
    }
}
