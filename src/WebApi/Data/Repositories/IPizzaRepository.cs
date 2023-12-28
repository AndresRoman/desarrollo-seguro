using WebApi.Data.Models;

namespace WebApi.Data.Repositories;

public interface IPizzaRepository
{
    public Task<Pizza?> GetPizzaAsync(int id);
    public Task<List<Pizza>> GetPizzasAsync();
    public Task<Pizza> AddPizzaAsync(Pizza pizza);
    public Task<Pizza> UpdatePizzaAsync(Pizza pizza);
    public Task DeletePizzaAsync(int id);

}