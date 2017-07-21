using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AP.Server.Application
{
    public class CommaDelimitedArrayModelBinder : IModelBinder
    {
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelMetadata.IsEnumerableType)
            {
                var key = bindingContext.ModelName;
                var value = bindingContext.ValueProvider.GetValue(key).ToString();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
                    var converter = TypeDescriptor.GetConverter(elementType);

                    var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => converter.ConvertFromString(x.Trim()))
                        .ToArray();

                    var typedValues = Array.CreateInstance(elementType, values.Length);

                    values.CopyTo(typedValues, 0);

                    bindingContext.Model = typedValues;
                    bindingContext.Result = ModelBindingResult.Success(typedValues);
                }
                else
                {
                    // change this line to null if you prefer nulls to empty arrays 
                    bindingContext.Model = Array.CreateInstance(bindingContext.ModelType.GetElementType(), 0);
                }
            }
            return Task.CompletedTask;
        }
    }
}
