using Microsoft.AspNetCore.Mvc;
using SteamCurrencyAPI.Interfaces;
using SteamCurrencyAPI.Models;

namespace SteamCurrencyAPI.Controllers;

[ApiController]
[Route("api/currency")]
public class CurrencyController(ICurrencyService currencyService) : ControllerBase
{
    [HttpGet("latest-rate")]
    public async Task<IActionResult> GetLatestRate([FromQuery] LastestRateRequest latestRateRequest)
    {
        try
        {
            var latestRate = await currencyService.GetLatestRate(latestRateRequest);

            return Ok(new
            {
                latestRate.CurrencyCode,
                latestRate.CurrencyBaseCode,
                CurrencyInfo = new CurrencyInfo
                {
                    DateAtUtc = DateOnly.FromDateTime(latestRate.CurrencyInfo.DateAtUtc),
                    CurrencyPrice = latestRate.CurrencyInfo.CurrencyPrice
                }
            });
        }
        catch (Exception e)
        {
            return BadRequest($"Error getting last currency rate.");
        }
    }


    [HttpGet("rates")]
    public async Task<IActionResult> GetRate([FromQuery] RateRequest rateRequest)
    {
        var rate = await currencyService.GetRate(rateRequest);

        return Ok(rate);
    }

    [HttpGet("codes")]
    public async Task<IActionResult> GetAllCurrencyCodes()
    {
        var codes = await currencyService.GetAllCurrencyCodes();

        return Ok(codes);
    }
}