using System;
using System.Collections.Generic;
using System.Text;

namespace Ravency.Application.Shared.DTO
{
    public class LanguageDto<T>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public T Data { get; set; }
    }
}
