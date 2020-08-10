using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BusinessObjects
{
    public class BaseModelForFiltering<T>
    {
        public int TotalCount { get; set; }
        public IQueryable<T> Results { get; set; }
    }
}
