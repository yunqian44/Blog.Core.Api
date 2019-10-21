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

        public static bool ObjToBool(this object thisValue)
        {
            bool reval = false;
            if (thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        }

        public static DateTime ObjToDate(this object thisValue)
        {
            DateTime reval = DateTime.MinValue;
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
            {
                reval = Convert.ToDateTime(thisValue);
            }
            return reval;
        }

    }
}
