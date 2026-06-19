using System.Reflection;
using Evertech.Overtime.Domain.Attributes;
using Evertech.Overtime.Domain.Entities.Base;

namespace Evertech.Overtime.Infrastructure.Data;

internal static class UpdateBuilder
{
    private static readonly Dictionary<Type, PropertyInfo[]> PropertyCache = [];

    public static UpdateCommand? Build<T>(T current, T updated) where T : Entity
    {
        var properties = GetUpdatableProperties<T>();
        var changes = new Dictionary<string, object?>();

        foreach (var property in properties)
        {
            var currentValue = property.GetValue(current);
            var updatedValue = property.GetValue(updated);

            if (!Equals(currentValue, updatedValue))
                changes[property.Name] = updatedValue;
        }

        if (changes.Count == 0)
            return null;

        var setClause = string.Join(", ", changes.Keys.Select(name => $"{name} = @{name}"));
        var parameters = new Dictionary<string, object?>(changes) { ["Id"] = current.Id };

        return new UpdateCommand(setClause, parameters);
    }

    private static PropertyInfo[] GetUpdatableProperties<T>() where T : Entity
    {
        var type = typeof(T);

        if (PropertyCache.TryGetValue(type, out var cached))
            return cached;

        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetCustomAttribute<NotUpdatableAttribute>() is null)
            .Where(property => property.CanRead)
            .ToArray();

        PropertyCache[type] = properties;
        return properties;
    }
}

internal sealed record UpdateCommand(string SetClause, IReadOnlyDictionary<string, object?> Parameters);