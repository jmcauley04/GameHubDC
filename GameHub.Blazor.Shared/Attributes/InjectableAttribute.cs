namespace GameHub.Blazor.Shared.Attributes;

internal class InjectableAttribute : Attribute
{
    private readonly InjectionType _injectType;

    internal enum InjectionType
    {
        Transient,
        Scoped,
        Singleton
    }

    public InjectableAttribute(InjectionType injectType)
    {
        _injectType = injectType;
    }

    public InjectionType Type => _injectType;
}
