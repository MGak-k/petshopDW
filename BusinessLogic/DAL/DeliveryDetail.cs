//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BusinessLogic.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class DeliveryDetail
    {
        public System.Guid DeliveryDetailsID { get; set; }
        public Nullable<System.Guid> MemberID { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Nullable<System.Guid> OrderID { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public string PaymentType { get; set; }
    
        public virtual Member Member { get; set; }
    }
}
