namespace Exlibris.Core.Reflection;
public static class ReflectionUtil
{
    public static Type GetType(string typeName) => Type.GetType(typeName) ?? throw new ArgumentException($"can not resolive the type of {typeName}");
       
}
