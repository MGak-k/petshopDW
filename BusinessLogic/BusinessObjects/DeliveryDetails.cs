using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BusinessObjects
{
   public class DeliveryDetails
    {
        public System.Guid DeliveryDetailsID { get; set; }
        public Nullable<System.Guid> PaymentItem { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Nullable<System.Guid> OrderID { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public string PaymentType { get; set; }
        public Nullable<bool> IsSent { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
    }
}
