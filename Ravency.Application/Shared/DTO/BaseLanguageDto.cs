using System;
using System.Collections.Generic;
using System.Text;

namespace Ravency.Application.Shared.DTO
{
    public class BaseLanguageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool IsDefault { get; set; }
    }
}
