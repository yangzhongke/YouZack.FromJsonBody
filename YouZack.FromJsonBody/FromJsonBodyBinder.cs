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
                object jsonValue = jsonProperty.GetValue();
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

        
    }
}
