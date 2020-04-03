using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YouZack.FromJsonBody
{
    public sealed class FromJsonBodyMiddleware
    {
        public const string RequestJsonObject_Key = "RequestJsonObject";

        private readonly RequestDelegate _next;

        public FromJsonBodyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if(!Helper.ContentTypeIsJson(context, out string charSet))
            {
                await _next(context);
                return;
            }
            Encoding encoding = Encoding.GetEncoding(charSet);
            context.Request.EnableBuffering();//Ensure the HttpRequest.Body can be read multipletimes
            int contentLen = 255;
            if (context.Request.ContentLength != null)
            {
                contentLen = (int)context.Request.ContentLength;
            }
            Stream body = context.Request.Body;
            using (StreamReader reader = new StreamReader(body, encoding, false, contentLen, false))
            {
                //parse json into JsonElement in advance,
                //to reduce multiple times of parseing in FromJsonBodyBinder.BindModelAsync
                string json = await reader.ReadToEndAsync();
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    body.Position = 0;
                    var root = document.RootElement;
                    context.Items.Add(RequestJsonObject_Key, root);
                    await _next(context);
                }
            }
        }
    }
}
