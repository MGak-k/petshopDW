//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BusinessLogic.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class DeliveryDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DeliveryDetail()
        {
            this.PaymentItems = new HashSet<PaymentItem>();
        }
    
        public System.Guid DeliveryDetailsID { get; set; }
        public Nullable<System.Guid> PaymentItem { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Nullable<System.Guid> OrderID { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public string PaymentType { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentItem> PaymentItems { get; set; }
    }
}
