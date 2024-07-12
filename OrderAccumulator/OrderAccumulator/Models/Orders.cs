namespace OrderAccumulatorApp.Models;

public class Orders
{
    public int Id {get; set;}
    public string Symbol {get; set;}
    public string Side {get; set;}
    public string ExecType {get; set;}
    public int OrderQty {get; set;}
    public decimal Price {get; set;}
    public decimal OrderTotal {get; set;}
}