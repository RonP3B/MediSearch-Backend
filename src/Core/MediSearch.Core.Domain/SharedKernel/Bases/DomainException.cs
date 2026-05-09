namespace MediSearch.Core.Domain.SharedKernel.Bases;

public abstract class DomainException(string errorCode) : Exception(errorCode) { }
