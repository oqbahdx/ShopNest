using ShopNest.Application.Features.Categories.Commands.CreateCategory;

namespace ShopNest.UnitTests.Validators;

public sealed class CategoryValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public async Task Valid_command_passes_validation()
    {
        var result = await _validator.ValidateAsync(new CreateCategoryCommand("Laptops", null, null, 0, null));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Empty_name_fails_validation(string name)
    {
        var result = await _validator.ValidateAsync(new CreateCategoryCommand(name, null, null, 0, null));
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCategoryCommand.Name));
    }

    [Fact]
    public async Task Empty_parent_guid_and_negative_display_order_fail_validation()
    {
        var result = await _validator.ValidateAsync(new CreateCategoryCommand("Sub", null, null, -1, Guid.Empty));

        result.Errors.Select(e => e.PropertyName).Should().Contain([
            nameof(CreateCategoryCommand.ParentCategoryId),
            nameof(CreateCategoryCommand.DisplayOrder)
        ]);
    }
}
