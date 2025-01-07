using System.Collections.Concurrent;
using System.Text.Json;
using demo_discounts_api.Helpers;
using demo_discounts_api.Models;
using demo_discounts_api.Repositories.Contracts;

namespace demo_discounts_api.Repositories
{
    public class DiscountCodeFileRepository : IDiscountCodeRepository
    {
        private readonly string _filePath;
        private static readonly object FileLock = new();

        public DiscountCodeFileRepository(string relativePath = "data/discount_codes.json")
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            // Ensure the directory exists
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Initialize the file if it doesn't exist
            if (!File.Exists(_filePath))
            {
                SaveToFile(new ConcurrentDictionary<string, DiscountCode>());
            }
        }

        public async Task<List<string>> GenerateCodes(int count, int length)
        {
            var generatedCodes = new List<string>();

            lock (FileLock)
            {
                var discountCodes = LoadFromFile();

                for (int i = 0; i < count; i++)
                {
                    var code = CodeGenerationHelper.GenerateRandomCode(length);

                    if (discountCodes.ContainsKey(code))
                    {
                        while (discountCodes.ContainsKey(code))
                        {
                            code = CodeGenerationHelper.GenerateRandomCode(length);
                        }
                    }

                    var discount = new DiscountCode
                    {
                        Code = code,
                        DateCreated = DateTime.UtcNow,
                        DateUsed = null,
                        IsUsed = false
                    };

                    discountCodes.TryAdd(code, discount);
                    generatedCodes.Add(code);
                }

                SaveToFile(discountCodes);
            }

            return generatedCodes;
        }

        public async Task<bool> UseCode(string code)
        {
            lock (FileLock)
            {
                var discountCodes = LoadFromFile();

                if (!discountCodes.TryGetValue(code, out var discount))
                {
                    return false; // Code does not exist
                }

                if (discount.IsUsed)
                {
                    return false; // Code already used
                }

                // Mark the code as used
                discount.IsUsed = true;
                discount.DateUsed = DateTime.UtcNow;

                discountCodes[code] = discount;
                SaveToFile(discountCodes);

                return true;
            }
        }

        private ConcurrentDictionary<string, DiscountCode> LoadFromFile()
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<ConcurrentDictionary<string, DiscountCode>>(json)
                       ?? new ConcurrentDictionary<string, DiscountCode>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading file: {ex.Message}");
                return new ConcurrentDictionary<string, DiscountCode>();
            }
        }

        private void SaveToFile(ConcurrentDictionary<string, DiscountCode> discountCodes)
        {
            try
            {
                var json = JsonSerializer.Serialize(discountCodes, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
            }
        }
    }
}
