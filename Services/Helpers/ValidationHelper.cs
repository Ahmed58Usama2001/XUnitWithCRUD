using ServiceContracts.DTO;
using System.ComponentModel.DataAnnotations;

namespace Services.Helpers;

public class ValidationHelper
{
    internal static void ModelValidation(object model)
    {
        ValidationContext validationContext = new ValidationContext(model);
        List<ValidationResult> results = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(model, validationContext, results, true);

        if (!isValid)
        {
            throw new ArgumentException(results.FirstOrDefault()?.ErrorMessage);
        }
    }
}
