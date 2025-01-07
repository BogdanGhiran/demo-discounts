using demo_discounts_api.Repositories.Contracts;
using demo_discounts_api.Services.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace demo_discounts_api.Hubs;

public class DiscountHub : Hub
{
    private readonly IDiscountCodeService _codeService;

    public DiscountHub(IDiscountCodeService codeService)
    {
        _codeService = codeService;
    }

    public async Task GenerateCodes(int count, int length)
    {
        var response = await _codeService.GenerateCodes(count, length);
        
        await Clients.Caller.SendAsync("CodesGenerated", response.Success);
    }

    public async Task UseCode(string code)
    {
        var response = await _codeService.UseCode(code);
        
        await Clients.Caller.SendAsync("CodeUsed", new { Code = code, Success = response.Success });
    }
    
    //Extra method added to make testing easier
    public async Task GenerateCodesWithResponse(int count, int length)
    {
        var response = await _codeService.GenerateCodes(count, length);

        var codes = response.Payload;
        await Clients.Caller.SendAsync("CodesGeneratedWithResponse", codes);
    }

}