namespace MediSearch.Core.Domain.SharedKernel.Interfaces;

public interface ISmartEnum<TSelf>
    where TSelf : ISmartEnum<TSelf>
{
    int Id { get; }
    string Name { get; }

    static abstract TSelf GetById(int id);
    static abstract TSelf GetByName(string name);
    static abstract IReadOnlyCollection<TSelf> List();
}
