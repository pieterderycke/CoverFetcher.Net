using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoverFetcher
{
    [DataContract]
    public class SearchResult
    {
        [DataMember(Name = "resultCount")]
        public int ResultCount { get; set; }

        [DataMember(Name = "results")]
        public IEnumerable<SearchResultItem> Items { get; set; }
    }
}
