using System.Reflection;

namespace MediSearch.Infrastructure.Persistence;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
