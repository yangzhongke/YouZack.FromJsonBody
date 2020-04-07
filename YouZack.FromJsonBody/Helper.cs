using Microsoft.AspNetCore.Http;
using System;
using System.Net.Mime;
using System.Text.Json;

namespace YouZack.FromJsonBody
{
    static class  Helper
    {
        public static bool ContentTypeIsJson(HttpContext httpContext,out string charSet)
        {
            string strContentType = httpContext.Request.ContentType;
            if (string.IsNullOrEmpty(strContentType))
            {
                charSet = null;
                return false;
            }
            ContentType contentType = new ContentType(strContentType);
            charSet = contentType.CharSet;
            return contentType.MediaType.ToLower() == "application/json";
        }

        /// <summary>
        /// Get value of JsonElement
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>

        public static object GetValue(this JsonElement property)
        {
            switch (property.ValueKind)
            {
                case JsonValueKind.Array:
                    throw new NotSupportedException("Array is not supported, use [FromBody] and Model instead");
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.Number:
                    return property.GetDecimal();
                case JsonValueKind.Object:
                    throw new NotSupportedException("Object is not supported, use [FromBody] and Model instead");
                case JsonValueKind.String:
                    return property.GetString();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.Undefined:
                    return null;
                default:
                    throw new ArgumentException("Unkown property.ValueKind");
            }
        }

        //https://stackoverflow.com/questions/18015425/invalid-cast-from-system-int32-to-system-nullable1system-int32-mscorlib
        public static object ChangeType(this object value, Type conversion)
        {
            var t = conversion;
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }
                t = Nullable.GetUnderlyingType(t);
            }
            if (t.IsEnum)
            {
                string s = Convert.ToString(value);
                return Enum.Parse(t, s, true);
            }
            if(t==typeof(Guid))
            {
                string s = Convert.ToString(value);
                return Guid.Parse(s);
            }
            return Convert.ChangeType(value, t);
        }
    }
}
