namespace Product_MVC.Funtion;

public class VatCalculator
{
    private double VAT;
    public VatCalculator(IConfiguration configuration) => VAT = configuration.GetValue<double>("VATSettings:VATPercentage");
    public double CalculateTotalPriceWithVat(int quantiy, double price) => quantiy * price * (1 + VAT);
}
