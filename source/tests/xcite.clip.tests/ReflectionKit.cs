using System.Reflection;

namespace xcite.clip.tests; 

public static class ReflectionKit {
    
    public static object InvokeStaticMethod(Type type, string name, Type[] types, object[] args) {
        MethodInfo method = type.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic, null, types, null);
        return method.Invoke(null, args);
    }
    
}