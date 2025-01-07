namespace demo_discounts_api.Repositories.Contracts
{
    public interface IDiscountCodeRepository
    {
        public Task<bool> UseCode(string code);

        public Task<List<string>> GenerateCodes(int count, int length);
    }
}
