using Microsoft.EntityFrameworkCore;
using WebApi.Data.Models;

namespace WebApi.Data.Repositories;

public class PizzaRepository(ApplicationDbContext context) : IPizzaRepository
{
    public async Task<Pizza?> GetPizzaAsync(int id)
    {
        return await context.Pizzas!.FindAsync(id);
    }

    public async Task<List<Pizza>> GetPizzasAsync()
    {
        return await context.Pizzas!.ToListAsync();
    }

    public async Task<Pizza> AddPizzaAsync(Pizza pizza)
    {
        await context.Pizzas!.AddAsync(pizza);
        await context.SaveChangesAsync();
        return pizza;
    }

    public async Task<Pizza> UpdatePizzaAsync(Pizza pizza)
    {
        context.Pizzas!.Update(pizza);
        await context.SaveChangesAsync();
        return pizza;
    }

    public async Task DeletePizzaAsync(int id)
    {
        var pizza = await context.Pizzas!.FindAsync(id);
        context.Pizzas.Remove(pizza!);
        await context.SaveChangesAsync();
    }

}
