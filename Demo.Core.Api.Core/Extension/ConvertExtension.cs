using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Core.Extension
{
    public static class ConvertExtension
    {
        public static int ToInt(this object obj)
        {
            int reval = 0;
            if (obj == null) return 0;
            if (obj != null && obj != DBNull.Value && int.TryParse(obj.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        }

        public static string ObjToString(this object thisValue)
        {
            if (thisValue != null) return thisValue.ToString().Trim();
            return string.Empty;
        }
    }
}
