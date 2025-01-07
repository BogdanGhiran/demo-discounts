using demo_discounts_api.Helpers;
using demo_discounts_api.Models;
using demo_discounts_api.Repositories.Contracts;
using demo_discounts_api.Services.Contracts;
using System.Collections.Concurrent;

namespace demo_discounts_api.Repositories
{
    public class DiscountCodeMemoryRepository : IDiscountCodeRepository
    {

        private static readonly ConcurrentDictionary<string, DiscountCode> DiscountCodes = new();

        public async Task<List<string>> GenerateCodes(int count, int length)
        {
            List<string> codes = new List<string>();
            for (int i = 0; i < count; i++)
            {
                var code = CodeGenerationHelper.GenerateRandomCode(length);
                var discount = new DiscountCode
                {
                    Code = code,
                    DateCreated = DateTime.UtcNow,
                    DateUsed = null,
                    IsUsed = false
                };

                // TryAdd returns false if code already exists
                bool added = DiscountCodes.TryAdd(code, discount);

                if (added)
                {
                    codes.Add(code);
                }
            }
            return codes;
        }

        public async Task<bool> UseCode(string code)
        {
            if (!DiscountCodes.TryGetValue(code, out var discount))
            {
                return false;
            }

            lock (discount)
            {
                if (discount.IsUsed)
                {
                    return false;
                }
                
                discount.IsUsed = true;
                discount.DateUsed = DateTime.UtcNow;
                return true;
            }
        }
    }
}
