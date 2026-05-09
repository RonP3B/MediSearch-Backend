using FluentValidation;
using MediSearch.Core.Application.Catalog.Comments.Constants;
using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.Comments.Commands.AddCommentToProduct;

public sealed class AddCommentToProductCommandValidator
    : AbstractValidator<AddCommentToProductCommand>
{
    private readonly IProductRepository _productRepository;

    public AddCommentToProductCommandValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(v => v.Content).ValidValueObject(CleanText.TryFrom);

        RuleFor(v => v.ProductId)
            .ValidValueObject(EntityId<Product>.TryFrom)
            .DependentRules(() =>
                RuleFor(v => v.ProductId)
                    .MustAsync(ProductExists)
                    .WithCustomErrorCode(CommentErrorCodes.ProductDoesNotExist)
            );
    }

    private async Task<bool> ProductExists(Guid productId, CancellationToken cancellationToken)
    {
        return await _productRepository.ExistsAsync(
            EntityId<Product>.From(productId),
            cancellationToken
        );
    }
}
