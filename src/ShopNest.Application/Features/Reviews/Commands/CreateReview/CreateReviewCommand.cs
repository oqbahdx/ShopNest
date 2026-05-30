using MediatR;

namespace ShopNest.Application.Features.Reviews.Commands.CreateReview;

public sealed record CreateReviewCommand(
    Guid ProductId,
    int Rating,
    string Title,
    string Comment
) : IRequest<Result<Guid>>;