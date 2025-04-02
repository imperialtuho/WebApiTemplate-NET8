using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApiTemplate.Domain.Attributes
{
    /// <summary>
    /// Specifies that a parameter should be bound from a comma-separated values (CSV) string.
    /// </summary>
    /// <remarks>
    /// This attribute is used in ASP.NET Core model binding to automatically parse CSV input into an <see cref="IList{string}"/>.
    /// It is applied to action parameters to indicate that the parameter should be bound using <see cref="CsvModelBinder"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class CsvBinderAttribute : ModelBinderAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvBinderAttribute"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor specifies <see cref="CsvModelBinder"/> as the model binder responsible for handling CSV values.
        /// </remarks>
        public CsvBinderAttribute() : base(typeof(CsvModelBinder))
        {
        }
    }

    /// <summary>
    /// Provides custom model binding to parse comma-separated values (CSV) into a list of strings.
    /// </summary>
    /// <remarks>
    /// This model binder extracts the value from the request, splits it by commas, and converts it into an <see cref="IList{string}"/>.
    /// It is designed to be used with <see cref="CsvBinderAttribute"/> to enable automatic CSV parsing in ASP.NET Core action parameters.
    /// </remarks>
    public class CsvModelBinder : IModelBinder
    {
        /// <summary>
        /// Binds a comma-separated string from the request to an <see cref="IList{string}"/> model.
        /// </summary>
        /// <param name="bindingContext">The context of the model binding operation.</param>
        /// <returns>A task representing the asynchronous model binding operation.</returns>
        /// <remarks>
        /// This method retrieves the value from the request, checks if it exists, and splits it into a list of strings.
        /// If the input is null or empty, the binding is skipped.
        /// </remarks>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            string? value = valueProviderResult.FirstValue;

            if (string.IsNullOrWhiteSpace(value))
            {
                return Task.CompletedTask;
            }

            // Split the CSV values into a list
            string[]? values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // Set the model binding result
            bindingContext.Result = ModelBindingResult.Success(values.ToList());

            return Task.CompletedTask;
        }
    }
}