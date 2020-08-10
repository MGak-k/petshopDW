using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PetShopDW.Helpers
{
    public static class EmailTranslator
    {
        public static string Translate(string messageTemplate, Dictionary<string, string> dictionaryTerms)
        {
            var output = new StringBuilder(messageTemplate);

            foreach (var item in dictionaryTerms)
            {
                output.Replace(item.Key, item.Value);
            }

            return output.ToString();
        }

    }
}