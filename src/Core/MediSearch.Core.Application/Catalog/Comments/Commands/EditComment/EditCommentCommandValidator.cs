using FluentValidation;
using MediSearch.Core.Domain.Catalog.Comments;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.Comments.Commands.EditComment;

public sealed class EditCommentCommandValidator : AbstractValidator<EditCommentCommand>
{
    public EditCommentCommandValidator()
    {
        RuleFor(v => v.CommentId).ValidValueObject(EntityId<Comment>.TryFrom);

        RuleFor(v => v.NewContent).ValidValueObject(CleanText.TryFrom);
    }
}
