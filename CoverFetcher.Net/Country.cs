using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoverFetcher
{
    public class Country
    {
        public Country(string label, string value)
        {
            Label = label;
            Code = value;
        }

        public string Label { get; private set; }
        public string Code { get; private set; }

        public override string ToString()
        {
            return Label;
        }
    }
}
