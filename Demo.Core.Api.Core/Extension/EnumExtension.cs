using System;
using System.ComponentModel;

namespace Demo.Core.Api
{
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum o)
        {
            Type enumType = o.GetType();
            string name = Enum.GetName(enumType, o);
            var customAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(enumType.GetField(name), typeof(DescriptionAttribute));
            if (customAttribute != null)
            {
                return customAttribute.Description;
            }
            return name;
        }
    }
}
