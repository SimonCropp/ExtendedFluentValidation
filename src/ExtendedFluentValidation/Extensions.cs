static class Extensions
{
    const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

    public static bool NotNullable(this PropertyInfo property)
    {
        if (!property.PropertyType.IsClass)
        {
            return false;
        }

        var nullability = property.GetNullability();
        return nullability == NullabilityState.NotNull;
    }

    public static bool AllowsEmpty(this MemberInfo property) =>
        property.GetCustomAttribute<AllowEmptyAttribute>() != null;

    public static List<PropertyInfo> GettableProperties<[DynamicMembers(DynamicTypes.PublicProperties | DynamicTypes.NonPublicProperties)] T>(IReadOnlyList<string>? exclusions)
    {
        var type = typeof(T);

        var properties = type.GetProperties(flags)
            .Where(_ => _.GetMethod != null &&
                        !_.IsCompilerGenerated());

        if (exclusions != null)
        {
            properties = properties.Where(_ => !exclusions.Contains(_.Name));
        }

        return properties.ToList();
    }

    static bool IsCompilerGenerated(this PropertyInfo info) =>
        info.GetCustomAttribute<CompilerGeneratedAttribute>() != null;

    public static bool IsString(this PropertyInfo property) =>
        property.PropertyType == typeof(string);

    public static bool IsCollection(this PropertyInfo property)
    {
        var type = property.PropertyType;

        if (type.IsAssignableTo<ICollection>())
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