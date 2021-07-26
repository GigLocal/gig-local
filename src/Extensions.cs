using System;

namespace GigLocal
{
    public static class Extensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (value?.Length <= maxLength)
            {
                return value;
            }
            return $"{value.Substring(0, Math.Min(value.Length, maxLength))}...";
        }
    }
}
