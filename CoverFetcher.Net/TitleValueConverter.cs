using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoverFetcher
{
    public class TitleValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int position = (int)values[0] + 1; // Human numbering
            int resultCount = (int)values[1];

            if (resultCount == 0)
                return "CoverFetcher";
            else
                return string.Format("CoverFetcher - Result {0} of {1}", position, resultCount);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
