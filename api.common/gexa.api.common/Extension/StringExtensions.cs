using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gexa.api.common.Extension
{
    public class StringExtensions
    {
        public class StringExtensions
        {
            public static long? ToLong(this string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                long longValue;
                var result = Int64.TryParse(value, out longValue);

                if (!result)
                {
                    return null;
                }

                return longValue;
            }
        }
    }
}
