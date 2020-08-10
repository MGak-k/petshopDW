using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BusinessObjects
{
   public class Product
    {
        public System.Guid ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<System.Guid> CategoryID { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> ProductCount { get; set; }
        public Nullable<decimal> Price { get; set; }
    }
}
