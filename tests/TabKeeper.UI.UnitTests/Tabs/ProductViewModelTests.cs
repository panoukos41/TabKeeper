using Core;
using DynamicData.Binding;
using FluentAssertions;
using System.ComponentModel;
using System.Reactive.Linq;
using TabKeeper.Tabs;

namespace TabKeeper.UI.UnitTests.Tabs;

public sealed class ProductViewModelTests
{
    // todo: Break below method to more tests.
    // todo: Move ViewModel tests to Core.UnitTests project

    [Fact]
    public void Should_Change_Product_Properties()
    {
        // Arrange
        var product = new Product
        {
            Id = Uuid.NewUuid(),
            Name = "Test",
            Price = 10,
            Quantity = 1
        };

        var viewModel = new ProductViewModel(product);

        var propertyChangedFired = false;
        var propertyChangingFired = false;
        var total = viewModel.Total;
        var name = viewModel.Name;
        Problem? problem = null;

        ((INotifyPropertyChanging)viewModel).PropertyChanging += (s, e) => propertyChangingFired = true;
        ((INotifyPropertyChanged)viewModel).PropertyChanged += (s, e) => propertyChangedFired = true;

        viewModel.WhenValueChanged(x => x.Name).Subscribe(x => name = x);
        viewModel.WhenValueChanged(x => x.Total).Subscribe(x => total = x);
        viewModel.WhenProblem.Subscribe(x => problem = x);

        // Act/Assert
        total.Should().Be(10);

        viewModel.Price = 20;
        total.Should().Be(20);

        viewModel.Quantity = 2;
        total.Should().Be(40);

        viewModel.Price = -1;
        viewModel.Total.Should().Be(0);
        problem.Should().NotBeNull();

        viewModel.Price = 20;
        viewModel.Total.Should().Be(40);

        viewModel.Quantity = -1;
        viewModel.Total.Should().Be(0);

        viewModel.Quantity = 2;
        viewModel.Total.Should().Be(40);

        product.Name.Should().Be("Test");
        product.Price.Should().Be(10);
        product.Quantity.Should().Be(1);

        propertyChangingFired.Should().BeTrue();
        propertyChangedFired.Should().BeTrue();
    }
}
