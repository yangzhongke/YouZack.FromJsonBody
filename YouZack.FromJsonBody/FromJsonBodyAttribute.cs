using Microsoft.AspNetCore.Mvc;
using System;

namespace YouZack.FromJsonBody
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromJsonBodyAttribute : ModelBinderAttribute
    {
        public FromJsonBodyAttribute() : base(typeof(FromJsonBodyBinder))
        {
        }
    }
}