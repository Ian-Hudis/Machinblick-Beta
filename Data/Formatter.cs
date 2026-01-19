using System.Globalization;

namespace Maschinblick.Data
{


    public class Formatter
    {

        public class DataItems
        {
            public string? State { get; set; }
            public int TimeAmount { get; set; }
        }

        public static string FormatAsMoney(object value)
        {
            return ((double)value).ToString("C0", CultureInfo.CreateSpecificCulture("en-EU"));
        }


        public static string FormatAsMonth(object value)
        {
            if (value != null)
            {
                return Convert.ToDateTime(value).ToString("MMM");
            }

            return string.Empty;
        }

        public static string FormatAstime(object value)
        {
            return ((DateTime)value).ToString("g", CultureInfo.CreateSpecificCulture("en-US"));
        }


        public static string FormatAsPercent(object value)  // used for pie chart
        {
            value= Math.Round(100 * (double)value,2); 
            return (value).ToString() + "%";
        }

    }
}
