using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoverFetcher
{
    [DataContract]
    public class SearchResultItem
    {
        [DataMember(Name = "artworkUrl100")]
        public string ArtworkUrl100 { get; set; }

        public string ArtworkUrl600
        { 
            get 
            {
                return ArtworkUrl100.Substring(0, ArtworkUrl100.Length - ".100x100-75.jpg".Length) + ".600x600-75.jpg";
            } 
        }
    }
}
