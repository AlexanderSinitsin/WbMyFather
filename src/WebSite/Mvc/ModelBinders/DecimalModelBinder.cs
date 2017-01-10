using System;
using System.Globalization;
using System.Web.Mvc;

namespace WebSite.Mvc.ModelBinders
{
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                if (bindingContext.ModelType == typeof(decimal?) && string.IsNullOrEmpty(valueResult.AttemptedValue))
                {
                    bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
                    return null;
                }

                //if with period use InvariantCulture
                if (valueResult.AttemptedValue.Contains("."))
                {
                    actualValue = Convert.ToDecimal(valueResult.AttemptedValue,
                        CultureInfo.InvariantCulture);
                }
                else
                {
                    //if with comma use CurrentCulture
                    actualValue = Convert.ToDecimal(valueResult.AttemptedValue,
                        CultureInfo.CurrentCulture);
                }
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}
