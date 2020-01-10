using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Core.Extension
{
    public static class StringExtension
    {
        public static bool ObjToBool(this string thisValue)
        {
            bool reval = false;
            if (!string.IsNullOrWhiteSpace(thisValue) && bool.TryParse(thisValue, out reval))
            {
                return reval;
            }
            return reval;
        }
    }
}
