using TabKeeper.Tabs;

namespace TabKeeper.UI.Tabs;

public sealed class ProductViewModel : ViewModelBase
{
    private Product product;
    private int divisor = 0;

    public Uuid ProductId => product.Id;

    public string Name
    {
        get => product.Name;
        set => SetValidateAndRaise(ref product, product with { Name = value });
    }

    public decimal Price
    {
        get => product.Price;
        set => SetValidateAndRaise(ref product, product with { Price = value }, nameof(Price), nameof(Total));
    }

    public decimal Quantity
    {
        get => product.Quantity;
        set => SetValidateAndRaise(ref product, product with { Quantity = value }, nameof(Quantity), nameof(Total));
    }

    public int Divisor
    {
        get => divisor;
        set => SetAndRaise(ref divisor, value, nameof(Divisor), nameof(Total));
    }

    public decimal Total => product.Total;

    public ProductViewModel(Product product)
    {
        this.product = product;
    }
}
