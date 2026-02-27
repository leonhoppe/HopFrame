using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TestApplication.Models;

public class Typer {
    [Key]
    public Guid Id { get; set; }
    public int Number { get; set; }
    public bool Toggle { get; set; }
    public DateTime DateTime { get; set; }
    public DateOnly DateOnly { get; set; }
    public TimeOnly TimeOnly { get; set; }
    public ListSortDirection SortDirection { get; set; }
    public string? Text { get; set; }
    [EmailAddress]
    public string? Mail { get; set; }
    public string? LongText { get; set; }
    public string? Password { get; set; }
    public string? PhoneNumber { get; set; }
    public List<string> List { get; set; } = new();
    public List<ListSortDirection> SortDirections { get; set; } = new();
}