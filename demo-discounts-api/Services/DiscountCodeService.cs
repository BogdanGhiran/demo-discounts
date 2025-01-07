using demo_discounts_api.Constants;
using demo_discounts_api.Helpers;
using demo_discounts_api.Models;
using demo_discounts_api.Repositories.Contracts;
using demo_discounts_api.Services.Contracts;

namespace demo_discounts_api.Services
{
    public class DiscountCodeService : IDiscountCodeService
    {
        private readonly IDiscountCodeRepository _codeRepository;

        public DiscountCodeService(IDiscountCodeRepository codeRepository)
        {
            _codeRepository = codeRepository;
        }

        public async Task<Response<List<string>>> GenerateCodes(int count, int length)
        {
            if (count < ApplicationLimits.MinCount || count > ApplicationLimits.MaxCount)
            {
                return new Response<List<string>>(success: false, reason:
                    ErrorMessageHelper.GenerateCountInputError(count), new List<string>());
            }

            if (length < ApplicationLimits.MinLength || length > ApplicationLimits.MaxLength)
            {
                return new Response<List<string>>(success: false, reason:
                    ErrorMessageHelper.GenerateLengthInputError(length), new List<string>());
            }

            var codes = await _codeRepository.GenerateCodes(count, length);

            return new Response<List<string>>(true, string.Empty, codes);
        }

        public async Task<Response<bool>> UseCode(string code)
        {
            int length = code.Length;
            if (length < ApplicationLimits.MinLength || length > ApplicationLimits.MaxLength)
            {
                return new Response<bool>(success: false, reason:
                    ErrorMessageHelper.GenerateLengthInputError(length), false);
            }

            bool success = await _codeRepository.UseCode(code);

            return new Response<bool>(success, string.Empty, success);
        }
    }
}
