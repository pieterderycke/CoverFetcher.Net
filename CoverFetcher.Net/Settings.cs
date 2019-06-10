using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoverFetcher
{
    public class SettingsConfig
    {
        public ExportSettingsConfig ExportSettings { get; set; }
        public ItunesSearchConfig ItunesSearch { get; set; }

        public class ExportSettingsConfig
        {
            public string DefaultFileNamePattern { get; set; }
        }

        public class ItunesSearchConfig
        {
            public string DefaultCountry { get; set; }
        }
    }
}
