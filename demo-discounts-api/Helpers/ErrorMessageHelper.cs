using demo_discounts_api.Constants;

namespace demo_discounts_api.Helpers
{
    public class ErrorMessageHelper
    {
        public static string GenerateCountInputError(int value)
        {
            return String.Format(ErrorMessageFormats.InvalidInputError, "count", value, ApplicationLimits.MinCount, ApplicationLimits.MaxCount);
        }
        public static string GenerateLengthInputError(int value)
        {
            return String.Format(ErrorMessageFormats.InvalidInputError, "length", value, ApplicationLimits.MinCount, ApplicationLimits.MaxCount);
        }
    }
}
