namespace WebApi.Services.Pizza;

using Data.Models;

public interface IPizzaService
{
    public Task<Pizza?> GetPizzaAsync(int id, string? userId = null);
    public Task<List<Pizza>> GetPizzasAsync(string? userId = null);
    public Task<Pizza> AddPizzaAsync(Pizza pizza, string? userId = null);
    public Task<Pizza> UpdatePizzaAsync(int id, Pizza pizza, string? userId = null);
    public Task DeletePizzaAsync(int id, string? userId = null);
}