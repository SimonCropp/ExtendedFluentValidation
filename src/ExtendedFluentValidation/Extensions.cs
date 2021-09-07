using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

static class Extensions
{
    const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

    public static List<PropertyInfo> GettableProperties<T>()
    {
        var type = typeof(T);
        return type.GetProperties(flags)
            .Where(_ => _.GetMethod != null)
            .ToList();
    }

    public static bool IsCollection(this Type type)
    {
        if (typeof(ICollection).IsAssignableFrom(type))
        {
            return true;
        }

        if (!type.IsGenericType)
        {
            return false;
        }

        var definition = type.GetGenericTypeDefinition();
        return definition == typeof(ICollection<>) ||
               definition == typeof(IReadOnlyCollection<>);
    }
}