using System.ComponentModel.DataAnnotations;

namespace AbiogenesisModel.Lib.Pipeline
{
    public static class ValidationHelper
    {
        public static void ValidateAndThrow<T>(T obj)
        {
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(obj!);
            var ok = Validator.TryValidateObject(obj!, ctx, results, validateAllProperties: true);
            if (!ok)
            {
                throw new ValidationException(
                    string.Join(Environment.NewLine, results.Select(r =>
                        $"{string.Join(".", r.MemberNames)}: {r.ErrorMessage}")));
            }
        }
    }
}
