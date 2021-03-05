using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace YouZack.FromJsonBody
{
    public class FromJsonBodyBinder : IModelBinder
    {
        public static readonly IDictionary<string, FromJsonBodyAttribute> fromJsonBodyAttrCache = new ConcurrentDictionary<string, FromJsonBodyAttribute>();

        //BindModelAsync will be invoked on every parameter that is marked as [FromJsonBody]
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!Helper.ContentTypeIsJson(bindingContext.HttpContext, out string charSet))
            {
                throw new ApplicationException("ContentType of request should be application/json");
            }
            var key = FromJsonBodyMiddleware.RequestJsonObject_Key;
            object itemValue = bindingContext.ActionContext.HttpContext.Items[key];
            if (itemValue == null)
            {
                throw new ApplicationException("'RequestJsonObject' not found in HttpContext.Items,please app.UseMiddleware<FromJsonBodyMiddleware>() or app.UseFromJsonBody() first");
            }
            JsonElement jsonObj = (JsonElement)itemValue;
            string fieldName = bindingContext.FieldName;
            FromJsonBodyAttribute fromJsonBodyAttr = GetFromJsonBodyAttr(bindingContext, fieldName);
            //if the propertyName of FromJsonBodyAttribute is not null, use that value 
            // as the fieldName instanceof the parameter name
            //for example: [FromJsonBody("i2")] int i1
            if (!string.IsNullOrWhiteSpace(fromJsonBodyAttr.PropertyName))
            {
                fieldName = fromJsonBodyAttr.PropertyName;
            }

            object jsonValue;
            //if property found
            //bindingContext.FieldName is the name of binded parameter
            if (ParseJsonValue(jsonObj, fieldName, out jsonValue))
            {
                //conver to the type of jsonValue to  type of parameter
                //bindingContext.ModelType:parameter type
                object targetValue = jsonValue.ChangeType(bindingContext.ModelType);
                bindingContext.Result = ModelBindingResult.Success(targetValue);
            }
            else//if property not found
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <param name="fieldName">can be: "name" or "owner.name" or "owner.owner.age"</param>
        /// <param name="jsonValue"></param>
        /// <returns></returns>
        private static bool ParseJsonValue(JsonElement jsonObj, string fieldName, out object jsonValue)
        {
            int firstDotIndex = fieldName.IndexOf('.');
            //if [FromJsonBody("author.father.name")]
            if (firstDotIndex>=0)
            {
                //"author"
                string firstPropName = fieldName.Substring(0, firstDotIndex);
                //"father.name"
                string leftPart = fieldName.Substring(firstDotIndex + 1);
                if(jsonObj.TryGetProperty(firstPropName, out JsonElement firstElement))
                {
                    return ParseJsonValue(firstElement, leftPart, out jsonValue);
                }
                else
                {
                    jsonValue = null;
                    return false;
                }
            }
            else
            {
                bool b = jsonObj.TryGetProperty(fieldName, out JsonElement jsonProperty);
                if (b)
                {
                    jsonValue = jsonProperty.GetValue();
                }
                else
                {
                    jsonValue = null;
                }
                return b;
            }            
        }

        private static FromJsonBodyAttribute GetFromJsonBodyAttr(ModelBindingContext bindingContext, string fieldName)
        {
            var actionDesc = bindingContext.ActionContext.ActionDescriptor;
            string actionId = actionDesc.Id;
            string cacheKey = $"{actionId}:{fieldName}";

            //fetch from cache to improve performance
            FromJsonBodyAttribute fromJsonBodyAttr;
            if (!fromJsonBodyAttrCache.TryGetValue(cacheKey, out fromJsonBodyAttr))
            {
                var ctrlActionDesc = bindingContext.ActionContext.ActionDescriptor as ControllerActionDescriptor;
                var fieldParameter = ctrlActionDesc.MethodInfo.GetParameters().Single(p => p.Name == fieldName);
                fromJsonBodyAttr = fieldParameter.GetCustomAttributes(typeof(FromJsonBodyAttribute), false).Single() as FromJsonBodyAttribute;
                fromJsonBodyAttrCache[cacheKey] = fromJsonBodyAttr;
            }            
            return fromJsonBodyAttr;
        }
    }
}
