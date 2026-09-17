using OrderFlow.Domain.Entities;

namespace OrderFlow.UnitTests.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesProduct()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Samsung Monitor";
        var description = "Full HD monitor";
        var price = 300m;
        var stock = 20;

        // Act
        var product = new Product(
            id,
            name,
            description,
            price,
            stock);

        // Assert
        Assert.Equal(id, product.Id);
        Assert.Equal(name, product.Name);
        Assert.Equal(description, product.Description);
        Assert.Equal(price, product.Price);
        Assert.Equal(stock, product.Stock);
    }

    [Fact]
    public void Constructor_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var act = () => new Product(
            id,
            "",
            null,
            100m,
            1);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithNegativePrice_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var act = () => new Product(
            id,
            "Product",
            null,
            -1m,
            1);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Constructor_WithNegativeStock_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var act = () => new Product(
            id,
            "Product",
            null,
            100m,
            -1);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Update_WithValidData_UpdatesProduct()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Mouse",
            "Description",
            1000m,
            5);

        // Act
        product.Update(
            "Keyboard",
            "New description",
            1500m,
            10);

        // Assert
        Assert.Equal("Keyboard", product.Name);
        Assert.Equal("New description", product.Description);
        Assert.Equal(1500m, product.Price);
        Assert.Equal(10, product.Stock);
    }

    [Fact]
    public void Update_WithNegativeStock_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Mouse",
            null,
            1000m,
            5);

        // Act
        var act = () => product.Update(
            "Mouse",
            null,
            1000m,
            -1);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }
}