using System.Collections;
using System.Globalization;

namespace NanosSharp.Interop.Values;

internal static class ValueConverter
{
    private static readonly Type[] ListDefinitions =
    [
        typeof(List<>), typeof(IList<>), typeof(IReadOnlyList<>), typeof(IEnumerable<>), typeof(ICollection<>),
        typeof(IReadOnlyCollection<>),
    ];

    private static readonly Type[] DictionaryDefinitions =
    [
        typeof(Dictionary<,>), typeof(IDictionary<,>), typeof(IReadOnlyDictionary<,>),
    ];

    public static T? Convert<T>(object? value) => (T?)Convert(value, typeof(T));

    public static object? Convert(object? value, Type targetType)
    {
        if (targetType == typeof(object) || targetType == typeof(void))
        {
            return value;
        }

        var underlyingType = Nullable.GetUnderlyingType(targetType);
        if (value is null)
        {
            return DefaultOf(targetType, underlyingType is not null);
        }

        targetType = underlyingType ?? targetType;
        if (targetType.IsInstanceOfType(value))
        {
            return value;
        }

        if (targetType.IsEnum)
        {
            return Enum.ToObject(targetType, System.Convert.ToInt64(value, CultureInfo.InvariantCulture));
        }

        if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(targetType))
        {
            return System.Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }

        return value switch
        {
            object?[] items when targetType.IsArray => ConvertArray(items, targetType.GetElementType()!),
            object?[] items when IsGenericOf(targetType, ListDefinitions) => ConvertList(items, targetType),
            Dictionary<object, object?> map when IsGenericOf(targetType, DictionaryDefinitions) => ConvertDictionary(map, targetType),
            _ => throw new InvalidCastException($"Cannot convert {value.GetType()} to {targetType}."),
        };
    }

    private static object? DefaultOf(Type targetType, bool isNullable) =>
        targetType.IsValueType && !isNullable ? Activator.CreateInstance(targetType) : null;

    private static bool IsGenericOf(Type type, Type[] definitions) =>
        type.IsGenericType && definitions.Contains(type.GetGenericTypeDefinition());

    private static Array ConvertArray(object?[] items, Type elementType)
    {
        var result = Array.CreateInstance(elementType, items.Length);
        for (var index = 0; index < items.Length; index++)
        {
            result.SetValue(Convert(items[index], elementType), index);
        }

        return result;
    }

    private static IList ConvertList(object?[] items, Type targetType)
    {
        var elementType = targetType.GetGenericArguments()[0];
        var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;
        foreach (var item in items)
        {
            list.Add(Convert(item, elementType));
        }

        return list;
    }

    private static IDictionary ConvertDictionary(Dictionary<object, object?> map, Type targetType)
    {
        var arguments = targetType.GetGenericArguments();
        var dictionary = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(arguments))!;
        var entries = map.Select(entry => (Key: Convert(entry.Key, arguments[0]), entry.Value)).Where(entry => entry.Key is not null);
        foreach (var (key, value) in entries)
        {
            dictionary[key!] = Convert(value, arguments[1]);
        }

        return dictionary;
    }
}
