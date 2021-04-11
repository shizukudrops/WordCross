using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCross
{
    public class DictionaryInfo
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string Separator { get; set; }

        public DictionaryInfo(string name, string baseUrl) 
        {
            Name = name;
            BaseUrl = baseUrl;
        }

        public DictionaryInfo(string name, string baseUrl, string separator)
        {
            Name = name;
            BaseUrl = baseUrl;
            Separator = separator;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
