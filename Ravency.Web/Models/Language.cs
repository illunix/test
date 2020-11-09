using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ravency.Web.Models
{
    public class Language<T>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public T Data { get; set; }
    }
}
