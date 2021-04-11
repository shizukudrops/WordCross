using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WordCross
{
    [JsonObject]
    public class DictionaryInfo
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string BaseUri { get; set; }
        [JsonProperty]
        public string Separator { get; set; }

        public DictionaryInfo() { }

        public DictionaryInfo(string name, string baseUrl) 
        {
            Name = name;
            BaseUri = baseUrl;
        }

        public DictionaryInfo(string name, string baseUri, string separator)
        {
            Name = name;
            BaseUri = baseUri;
            Separator = separator;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
