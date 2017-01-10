using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebSite.Mvc.ModelBinders
{
    public class DictionaryStringStringModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var keys = controllerContext
                .HttpContext
                .Request
                .Params
                .Keys
                .OfType<string>()
                .Where(key => key.StartsWith(modelName));

            var result = new Dictionary<string, string>();
            foreach (var key in keys)
            {
                var val = bindingContext.ValueProvider.GetValue(key);
                result[key.Replace(modelName, "").Replace("[", "").Replace("]", "")] = val.AttemptedValue;
            }

            return result;
        }
    }
}
