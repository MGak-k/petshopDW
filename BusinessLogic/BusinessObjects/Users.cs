using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BusinessObjects
{
   public class Users
    {
        public System.Guid ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public System.Guid UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool Active { get; set; }
        public int[] UserGroupID { get; set; }

        public bool UserNameChanged { get; set; }
    }
}
