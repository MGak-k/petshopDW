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
    
    public partial class Cart
    {
        public System.Guid CartID { get; set; }
        public Nullable<System.Guid> ProductID { get; set; }
        public Nullable<System.Guid> MemberID { get; set; }
        public Nullable<int> CartStatusID { get; set; }
    
        public virtual Product Product { get; set; }
    }
}
