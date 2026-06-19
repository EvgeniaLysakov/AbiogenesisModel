using System.ComponentModel.DataAnnotations;
using AbiogenesisModel.Lib.Guard;

namespace AbiogenesisModel.Lib.Pipeline;

public static class ValidationHelper
{
    public static void ValidateAndThrow<T>(T obj)
    {
        Ensure.That(obj).IsNotNull();

        var results = new List<ValidationResult>();
        var ctx = new ValidationContext(obj!);
        var ok = Validator.TryValidateObject(obj!, ctx, results, validateAllProperties: true);
        if (ok)
        {
            return;
        }

        var errorMessages = results.Select(r => $"{string.Join(".", r.MemberNames)}: {r.ErrorMessage}");
        throw new ValidationException(string.Join(Environment.NewLine, errorMessages));
    }
}