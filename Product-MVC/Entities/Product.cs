using System.ComponentModel.DataAnnotations;

namespace Product_MVC.Entities;
public class Product
{
    public int Id { get; set; }
    [Required(ErrorMessage = "This product is available")]
    public string ItemName { get; set; }
    [Range(0,double.MaxValue, ErrorMessage = "The Amount cannot be negative")]
    public double Quantiy { get; set; }
	[Range(0, double.MaxValue, ErrorMessage = "The Price cannot be negative")]
	public double Price { get; set; }
    public double TotalPrice { get; set; }
}
