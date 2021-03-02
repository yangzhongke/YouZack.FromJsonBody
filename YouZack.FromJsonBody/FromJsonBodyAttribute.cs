using Microsoft.AspNetCore.Mvc;
using System;

namespace YouZack.FromJsonBody
{
    [AttributeUsage(AttributeTargets.Parameter,AllowMultiple =false,Inherited =false)]
    public class FromJsonBodyAttribute : ModelBinderAttribute
    {
        public string PropertyName { get; private set; }

        public FromJsonBodyAttribute(string propertyName=null) : base(typeof(FromJsonBodyBinder))
        {
            this.PropertyName = propertyName;
        }
    }
}