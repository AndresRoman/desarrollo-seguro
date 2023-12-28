using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Models;
using WebApi.Services.Pizza;

namespace WebApi.Controllers;

public class PizzaController(IPizzaService pizzaService) : BaseController
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
    {
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var pizzas = await pizzaService.GetPizzasAsync(userId);
        return Ok(pizzas);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var pizza = await pizzaService.GetPizzaAsync(id, userId);
        if (pizza == null)
        {
            return NotFound();
        }

        return Ok(pizza);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post(Pizza pizza)
    {
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await pizzaService.AddPizzaAsync(pizza, userId);
        return CreatedAtAction(nameof(Get), new { id = pizza.Id }, pizza);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Put(int id, Pizza pizza)
    {
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await pizzaService.UpdatePizzaAsync(id, pizza, userId);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await pizzaService.DeletePizzaAsync(id, userId);
        return Ok();
    }
}