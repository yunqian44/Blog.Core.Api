using System;
using System.ComponentModel;

namespace Demo.Core.Api.Core.Extension
{
    public static class EnumExtension
    {
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
