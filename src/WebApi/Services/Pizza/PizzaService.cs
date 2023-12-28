using Microsoft.AspNetCore.Identity;
using WebApi.Data.Models;
using WebApi.Data.Repositories;

namespace WebApi.Services.Pizza;

public class PizzaService(IPizzaRepository pizzaRepository, UserManager<User> userManager): IPizzaService
{
    

    public async Task<Data.Models.Pizza?> GetPizzaAsync(int id, string? userId = null)
    {
        
        var user = await userManager.FindByIdAsync(userId!);
        var role = await userManager.GetRolesAsync(user!);
        var pizza = await pizzaRepository.GetPizzaAsync(id);
        if (pizza!.CreatedBy == user?.Id || role.Contains("Admin"))
        {
            return pizza;
        }

        return null;
    }

    public async Task<List<Data.Models.Pizza>> GetPizzasAsync(string? userId = null)
    {
        var user = await userManager.FindByIdAsync(userId!);
        var role = await userManager.GetRolesAsync(user!);
        var pizzas = await pizzaRepository.GetPizzasAsync();
        if (role.Contains("Admin"))
        {
            return pizzas;
        }

        var pizzasUser = pizzas.Where(p => p.CreatedBy == user?.Id).ToList();
        return pizzasUser;
    }

    public async Task<Data.Models.Pizza> AddPizzaAsync(Data.Models.Pizza pizza, string? userId = null)
    {
        
        var user = await userManager.FindByIdAsync(userId!);
        pizza.CreatedBy = user?.Id;
        return await pizzaRepository.AddPizzaAsync(pizza);
    }

    public async Task<Data.Models.Pizza> UpdatePizzaAsync(int id, Data.Models.Pizza pizza, string? userId = null)
    {
        
        var user = await userManager.FindByIdAsync(userId!);
        var role = await userManager.GetRolesAsync(user!);
        
        if (pizza!.CreatedBy == user?.Id || role.Contains("Admin"))
        {
            return await pizzaRepository.UpdatePizzaAsync(pizza);
        }

        return pizza;
    }

    public async Task DeletePizzaAsync(int id, string? userId = null)
    {
        var user = await userManager.FindByIdAsync(userId!);
        var role = await userManager.GetRolesAsync(user!);
        var pizza = await pizzaRepository.GetPizzaAsync(id);
        if (pizza!.CreatedBy == user?.Id || role.Contains("Admin"))
        {
            await pizzaRepository.DeletePizzaAsync(id);
        }
    }
}