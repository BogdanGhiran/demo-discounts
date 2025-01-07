using Microsoft.AspNetCore.SignalR;

namespace demo_discounts_api.Hubs;

public class DiscountHub : Hub
{
    private static readonly HashSet<string> DiscountCodes = new HashSet<string>();
    
    private static readonly Random Random = new Random();
    
    public async Task GenerateCodes(int count, int length)
    {
        var codes = new List<string>();
        for (int i = 0; i < count; i++)
        {
            var code = GenerateRandomCode(length);
            DiscountCodes.Add(code);
            codes.Add(code);
        }
        
        await Clients.Caller.SendAsync("CodesGenerated", codes);
    }
    
    public async Task UseCode(string code)
    {
        bool success = false;
        if (DiscountCodes.Contains(code))
        {
            DiscountCodes.Remove(code);
            success = true;
        }
        
        await Clients.Caller.SendAsync("CodeUsed", new { Code = code, Success = success });
    }

    private string GenerateRandomCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(
            Enumerable.Range(0, length)
                .Select(_ => chars[Random.Next(chars.Length)])
                .ToArray()
        );
    }
}