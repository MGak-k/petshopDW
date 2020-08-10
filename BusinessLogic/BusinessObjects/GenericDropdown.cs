using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BusinessObjects
{
    public class GenericDropdown
    {
        public Guid id { get; set; }
        public string text { get; set; }
        public string code { get; set; }
        public bool disabled { get; set; }

    }
}
