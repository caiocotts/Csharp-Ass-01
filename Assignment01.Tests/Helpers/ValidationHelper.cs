using System.ComponentModel.DataAnnotations;

namespace Assignment01.Tests.Helpers;

public static class ValidationHelper
{
    public static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, validationResults, validateAllProperties: true);
        return validationResults;
    }

    public static bool HasValidationError(IList<ValidationResult> results, string propertyName)
    {
        return results.Any(r => r.MemberNames.Contains(propertyName));
    }
}
