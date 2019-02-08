using System;
using System.ComponentModel;
using System.Reflection;

namespace Demo.Core.Api.Core
{
    public static class PropertyExtension
    {
        public static string GetDisplayName(this PropertyInfo property)
        {
            string result = property.Name;
            var attr = property.GetCustomAttribute<DisplayNameAttribute>(true);
            if (attr != null)
            {
                result = attr.DisplayName;
            }
            return result;
        }

        /// <summary>
        /// 获取属性描述
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetDescription(this PropertyInfo property)
        {
            string result = string.Empty;
            var attr = property.GetCustomAttribute<DescriptionAttribute>(true);
            if (attr != null)
            {
                result = attr.Description;
            }
            return result;
        }
    }
}