using System.ComponentModel.DataAnnotations;

namespace blazor.Model;

public class Pizza
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
    public string Name { get; set; } = null!;
    public PizzaSize Size { get; set; } = PizzaSize.Small;
    public bool IsGlutenFree { get; set; }
    [Range(1, double.MaxValue, ErrorMessage = "Por favor, introduce un precio válido.")]
    public decimal Price { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}

public enum PizzaSize
{
    Small,
    Medium,
    Large
}