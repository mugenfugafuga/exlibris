namespace Exlibris.Core;

public static class ObjectRegistryFactory
{
    public static IObjectRegistry NewConcurrentObjectRegistry()
        => new ConcurrentObjectRegistry();
}
