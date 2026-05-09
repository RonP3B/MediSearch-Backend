namespace MediSearch.Core.Domain.SharedKernel.Exceptions;

public sealed class BusinessRuleException(string propertyName, ErrorCode errorCode)
    : DomainException(errorCode.Key)
{
    public string PropertyName { get; } = propertyName;
    public ErrorCode ErrorCode { get; } = errorCode;
}
