using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Helpers
{
    public class FilterToUTCAttributeAttribute : System.Attribute
    {
        public bool convert;
        public FilterToUTCAttributeAttribute(bool convert)
        {
            this.convert = convert;
        }

        public virtual bool Convert
        {
            get { return convert; }
        }
    }
}
