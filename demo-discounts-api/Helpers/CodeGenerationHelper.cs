namespace demo_discounts_api.Helpers
{
    public class CodeGenerationHelper
    {
        private static readonly Random Random = new Random();

        public static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(
                Enumerable.Range(0, length)
                    .Select(_ => chars[Random.Next(chars.Length)])
                    .ToArray()
            );
        }
    }
}
