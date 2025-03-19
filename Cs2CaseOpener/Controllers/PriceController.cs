using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/prices")]
public class PriceController(PriceService priceService) : ControllerBase
{
    private readonly PriceService _priceService = priceService;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var prices = await _priceService.GetAll();
        return Ok(prices);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var price = await _priceService.GetById(id);
        return Ok(price);
    }
}