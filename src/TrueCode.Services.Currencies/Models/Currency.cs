namespace TrueCode.Services.Currencies.Models;

public record Currency
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public decimal Rate { get; set; }
}