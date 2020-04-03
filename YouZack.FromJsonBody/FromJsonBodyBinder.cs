using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace YouZack.FromJsonBody
{
    public class FromJsonBodyBinder : IModelBinder
    {
        //BindModelAsync will be invoked every parameter that is marked as [FromJsonBody]
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if(!Helper.ContentTypeIsJson(bindingContext.HttpContext,out string charSet))
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
            //if property found
            //(bindingContext.FieldName:parameter name
            if (jsonObj.TryGetProperty(bindingContext.FieldName, out JsonElement jsonProperty))
            {
                object jsonValue = GetValue(jsonProperty);
                //conver to the type of jsonValue to  type of parameter
                //bindingContext.ModelType:parameter type
                object targetValue = ChangeType(jsonValue, bindingContext.ModelType);
                bindingContext.Result = ModelBindingResult.Success(targetValue);
            }
            else//if property not found
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Get value of JsonElement
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>

        static object GetValue(JsonElement property)
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
        private static object ChangeType(object value, Type conversion)
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
            if(t.IsEnum)
            {
                return Enum.Parse(t, Convert.ToString(value),true);
            }
            return Convert.ChangeType(value, t);
        }
    }
}
