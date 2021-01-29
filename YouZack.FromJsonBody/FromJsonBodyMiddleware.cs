using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
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
        private ILogger<FromJsonBodyMiddleware> logger;

        public FromJsonBodyMiddleware(RequestDelegate next,
            ILogger<FromJsonBodyMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            //for request that has large body, EnableBuffering will reduce performance,
            //and generally the body of "application/json" is not too large,
            //so only EnableBuffering on contenttype="application/json"
            string method = context.Request.Method;
            if (!Helper.ContentTypeIsJson(context, out string charSet)
                ||"GET".Equals(method, StringComparison.OrdinalIgnoreCase))
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
            string bodyText;
            using (StreamReader reader = new StreamReader(body, encoding, true, contentLen, true))
            {
                //parse json into JsonElement in advance,
                //to reduce multiple times of parseing in FromJsonBodyBinder.BindModelAsync
                bodyText = await reader.ReadToEndAsync();                
            }
            //no request body
            if(string.IsNullOrWhiteSpace(bodyText))
            {
                await _next(context);
                return;
            }
            //not invalid json
            if(!(bodyText.StartsWith("{")&& bodyText.EndsWith("}")))
            {
                await _next(context);
                return;
            }
            
            try
            {
                using (JsonDocument document = JsonDocument.Parse(bodyText))
                {
                    body.Position = 0;
                    JsonElement jsonRoot = document.RootElement;
                    context.Items[RequestJsonObject_Key] = jsonRoot;
                    await _next(context);
                }
            }
            catch(JsonException ex)
            {
                logger.LogError(ex,"json解析失败:"+bodyText);
                //invalid json format
                await _next(context);
                return;
            }
        }
    }
}
