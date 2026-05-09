namespace MediSearch.Core.Domain.SharedKernel.Bases;

public sealed record DomainFailure(string PropertyName, ErrorCode ErrorCode);
