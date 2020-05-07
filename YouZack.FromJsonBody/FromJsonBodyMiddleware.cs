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
            //for request that has large body, EnableBuffering will reduce performance,
            //and generally the body of "application/json" is not too large,
            //so only EnableBuffering on contenttype="application/json"
            if (!Helper.ContentTypeIsJson(context, out string charSet))
            {
                await _next(context);
                return;
            }
            Encoding encoding;
            if(string.IsNullOrWhiteSpace(charSet))
            {
                encoding = Encoding.UTF8;
            }
            else
            {
                encoding = Encoding.GetEncoding(charSet);
            }     

            context.Request.EnableBuffering();//Ensure the HttpRequest.Body can be read multipletimes
            int contentLen = 255;
            if (context.Request.ContentLength != null)
            {
                contentLen = (int)context.Request.ContentLength;
            }
            Stream body = context.Request.Body;
            using (StreamReader reader = new StreamReader(body, encoding, true, contentLen, true))
            {
                //parse json into JsonElement in advance,
                //to reduce multiple times of parseing in FromJsonBodyBinder.BindModelAsync
                string json = await reader.ReadToEndAsync();
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    body.Position = 0;
                    var root = document.RootElement;
                    context.Items[RequestJsonObject_Key]= root;
                    await _next(context);
                }
            }
        }
    }
}
