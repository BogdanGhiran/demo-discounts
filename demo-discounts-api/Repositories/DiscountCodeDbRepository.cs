using demo_discounts_api.Data;
using demo_discounts_api.Helpers;
using demo_discounts_api.Models;
using demo_discounts_api.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace demo_discounts_api.Repositories
{
    public class DiscountCodeDbRepository : IDiscountCodeRepository
    {
        private readonly DemoDiscountsDbContext _context;

        public DiscountCodeDbRepository(DemoDiscountsDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GenerateCodes(int count, int length)
        {
            var generatedCodes = new List<string>();
            var newDiscounts = new Dictionary<string, DiscountCode>();
            var utcDate = DateTime.UtcNow;
            
            for (int i = 0; i < count; i++)
            {
                string code;
                code = CodeGenerationHelper.GenerateRandomCode(length);
                if (newDiscounts.ContainsKey(code))
                {
                    while (newDiscounts.ContainsKey(code))
                    {
                        code = CodeGenerationHelper.GenerateRandomCode(length);
                    }
                }

                generatedCodes.Add(code);
                newDiscounts.Add(code, new DiscountCode
                {
                    Code = code,
                    DateCreated = utcDate,
                    IsUsed = false
                });
            }

            try
            {
                // Perform a bulk insert
                await BatchInsertAsync(newDiscounts.Select(x => x.Value));
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error during bulk insert: {ex.Message}");
                throw; // Rethrow or handle as needed
            }

            return generatedCodes;
        }

        private async Task BatchInsertAsync(IEnumerable<DiscountCode> codes, int batchSize = 500)
        {
            var batches = codes.Chunk(batchSize);

            foreach (var batch in batches)
            {
                await _context.DiscountCodes.AddRangeAsync(batch);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> UseCode(string code)
        {
            // Use a transaction to ensure atomicity
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Find the discount code with a row-level lock
                var discount = await _context.DiscountCodes
                    .FromSqlRaw("SELECT * FROM \"DiscountCodes\" WHERE \"Code\" = {0} FOR UPDATE", code)
                    .FirstOrDefaultAsync();

                if (discount == null)
                {
                    await transaction.RollbackAsync();
                    return false; // Code does not exist
                }

                if (discount.IsUsed)
                {
                    await transaction.RollbackAsync();
                    return false; // Code is already used
                }

                // Mark the code as used
                discount.IsUsed = true;
                discount.DateUsed = DateTime.UtcNow;

                _context.DiscountCodes.Update(discount);
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error using code: {ex.Message}");
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
