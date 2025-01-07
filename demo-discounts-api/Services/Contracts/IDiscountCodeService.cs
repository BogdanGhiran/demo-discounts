using demo_discounts_api.Models;

namespace demo_discounts_api.Services.Contracts
{
    public interface IDiscountCodeService
    {
        public Task<Response<bool>> UseCode(string code);

        public Task<Response<List<string>>> GenerateCodes(int count, int length);
    }
}
