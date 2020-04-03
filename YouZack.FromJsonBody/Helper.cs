using Microsoft.AspNetCore.Http;
using System.Net.Mime;

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
    }
}
