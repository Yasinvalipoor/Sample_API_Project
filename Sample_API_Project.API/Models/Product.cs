﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Sample_API_Project.API.Models;

//2 ways to remove null :
//Use ? After type
//Use //#nullable disable

public class Product
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }

    [Required]
    public int CategoryId { get; set; }
    [JsonIgnore]
    public virtual Category? Category { get; set; }
}
