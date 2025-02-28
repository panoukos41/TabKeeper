using Core;
using DynamicData.Binding;
using FluentAssertions;
using System.Reactive.Linq;
using TabKeeper.People;
using TabKeeper.Tabs;

namespace TabKeeper.UI.UnitTests.People;

public sealed class PersonViewModelTests
{
    [Fact]
    public void Should_Change_Total_For_One_Person()
    {
        // Arrange
        var salad = new TabProductViewModel(new() { Id = Uuid.NewUuid(), Name = "Salad", Price = 5 });
        var chips = new TabProductViewModel(new() { Id = Uuid.NewUuid(), Name = "Chips", Price = 10 });
        var meat = new TabProductViewModel(new() { Id = Uuid.NewUuid(), Name = "Meat", Price = 15 });

        var viewModel = new TabPersonViewModel(new() { Id = Uuid.NewUuid(), Name = "Panos" });

        decimal total = 0;
        viewModel.WhenValueChanged(x => x.Total).Subscribe(x => total = x);

        // Act / Assert
        total.Should().Be(0);

        viewModel.RegisterProduct(salad);
        viewModel.RegisterProduct(salad);
        total.Should().Be(5);

        salad.Quantity = 2;
        total.Should().Be(10);

        viewModel.RegisterProduct(meat);
        total.Should().Be(25);

        viewModel.RegisterProduct(chips);
        total.Should().Be(35);

        chips.Quantity += 1;
        total.Should().Be(45);

        viewModel.UnRegisterProduct(chips);
        total.Should().Be(25);
    }

    [Fact]
    public void Should_Change_Totals_For_Many_People()
    {
        // Arrange
        var salad = new TabProductViewModel(new() { Id = Uuid.NewUuid(), Name = "Salad", Price = 6 });
        var chips = new TabProductViewModel(new() { Id = Uuid.NewUuid(), Name = "Chips", Price = 6 });
        var meat = new TabProductViewModel(new() { Id = Uuid.NewUuid(), Name = "Meat", Price = 15 });

        var panos = new TabPersonViewModel(new() { Id = Uuid.NewUuid(), Name = "Panos" });
        var john = new TabPersonViewModel(new() { Id = Uuid.NewUuid(), Name = "John" });
        var george = new TabPersonViewModel(new() { Id = Uuid.NewUuid(), Name = "George" });

        decimal panosTotal = 0, johnTotal = 0, georgeTotal = 0;

        panos.WhenValueChanged(x => x.Total).Subscribe(x => panosTotal = x);
        john.WhenValueChanged(x => x.Total).Subscribe(x => johnTotal = x);
        george.WhenValueChanged(x => x.Total).Subscribe(x => georgeTotal = x);

        var totals = () => (panosTotal, johnTotal, georgeTotal);

        // Act / Assert
        totals().Should().Be((0m, 0m, 0m));

        panos.RegisterProduct(salad);
        john.RegisterProduct(salad);
        totals().Should().Be((3m, 3m, 0m));
        salad.Divisor.Should().Be(2);

        salad.Quantity = 2;
        totals().Should().Be((6m, 6m, 0m));

        panos.RegisterProduct(chips);
        john.RegisterProduct(chips);
        george.RegisterProduct(chips);
        totals().Should().Be((8m, 8m, 2m));
        chips.Divisor.Should().Be(3);

        chips.Quantity = 2;
        totals().Should().Be((10m, 10m, 4m));

        panos.RegisterProduct(meat);
        john.RegisterProduct(meat);
        george.RegisterProduct(meat);
        totals().Should().Be((15m, 15m, 9m));
        meat.Divisor.Should().Be(3);

        george.RegisterProduct(salad);
        totals().Should().Be((13m, 13m, 13m));
        salad.Divisor.Should().Be(3);

        george.UnRegisterProduct(salad);
        totals().Should().Be((15m, 15m, 9m));
        salad.Divisor.Should().Be(2);

        decimal[] peopleTotals = [panos.Total, john.Total, george.Total];
        decimal[] productTotals = [salad.Total, chips.Total, meat.Total];
        peopleTotals.Sum().Should().Be(productTotals.Sum());

        george.Dispose();
        totals().Should().Be((19.5m, 19.5m, 0));
        chips.Divisor.Should().Be(2);
        meat.Divisor.Should().Be(2);
        peopleTotals = [panos.Total, john.Total];
        peopleTotals.Sum().Should().Be(productTotals.Sum());
    }

    [Fact]
    public void Should_Change_Totals_After_Removing_People_Or_Product()
    {
        // Arrange
        var salad = new TabProductViewModel(new() { Id = Uuid.NewUuid(), Name = "Salad", Price = 9 });

        var panos = new TabPersonViewModel(new() { Id = Uuid.NewUuid(), Name = "Panos" });
        var john = new TabPersonViewModel(new() { Id = Uuid.NewUuid(), Name = "John" });
        var george = new TabPersonViewModel(new() { Id = Uuid.NewUuid(), Name = "George" });

        decimal panosTotal = 0, johnTotal = 0, georgeTotal = 0;

        panos.WhenValueChanged(x => x.Total).Subscribe(x => panosTotal = x);
        john.WhenValueChanged(x => x.Total).Subscribe(x => johnTotal = x);
        george.WhenValueChanged(x => x.Total).Subscribe(x => georgeTotal = x);

        var totals = () => (panosTotal, johnTotal, georgeTotal);

        // Act / Assert
        totals().Should().Be((0m, 0m, 0m));

        panos.RegisterProduct(salad);
        john.RegisterProduct(salad);
        george.RegisterProduct(salad);
        totals().Should().Be((3m, 3m, 3m));
        salad.Divisor.Should().Be(3);

        // remove product
        john.UnRegisterProduct(salad);
        totals().Should().Be((4.5m, 0m, 4.5m));
        salad.Divisor.Should().Be(2);

        // simulate removing a person (which implies disposing the person)
        george.Dispose();
        totals().Should().Be((9m, 0m, 0m));
        salad.Divisor.Should().Be(1);

        // dispose product
        salad.Dispose();
        totals().Should().Be((0m, 0m, 0m));
        salad.Divisor.Should().Be(0);
    }
}
