using FluentValidation;
using MediSearch.Core.Domain.Catalog.Comments;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.Comments.Commands.AddCommentReply;

public sealed class AddCommentReplyCommandValidator : AbstractValidator<AddCommentReplyCommand>
{
    public AddCommentReplyCommandValidator()
    {
        RuleFor(v => v.Content).ValidValueObject(CleanText.TryFrom);

        RuleFor(v => v.CommentId).ValidValueObject(EntityId<Comment>.TryFrom);
    }
}
