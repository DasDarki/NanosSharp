using System.Collections;
using System.Globalization;

namespace NanosSharp.Interop.Values;

internal static class ValueConverter
{
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
            return targetType.IsValueType && underlyingType is null ? Activator.CreateInstance(targetType) : null;
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

        if (targetType.IsArray && value is object?[] array)
        {
            var elementType = targetType.GetElementType()!;
            var result = Array.CreateInstance(elementType, array.Length);
            for (var index = 0; index < array.Length; index++)
            {
                result.SetValue(Convert(array[index], elementType), index);
            }

            return result;
        }

        if (targetType.IsGenericType && value is object?[] items)
        {
            var definition = targetType.GetGenericTypeDefinition();
            if (definition == typeof(List<>) || definition == typeof(IList<>) || definition == typeof(IReadOnlyList<>) ||
                definition == typeof(IEnumerable<>) || definition == typeof(ICollection<>) ||
                definition == typeof(IReadOnlyCollection<>))
            {
                var elementType = targetType.GetGenericArguments()[0];
                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;
                foreach (var item in items)
                {
                    list.Add(Convert(item, elementType));
                }

                return list;
            }
        }

        if (targetType.IsGenericType && value is Dictionary<object, object?> map)
        {
            var definition = targetType.GetGenericTypeDefinition();
            if (definition == typeof(Dictionary<,>) || definition == typeof(IDictionary<,>) ||
                definition == typeof(IReadOnlyDictionary<,>))
            {
                var arguments = targetType.GetGenericArguments();
                var dictionary = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(arguments))!;
                foreach (var (key, entry) in map)
                {
                    if (Convert(key, arguments[0]) is { } convertedKey)
                    {
                        dictionary[convertedKey] = Convert(entry, arguments[1]);
                    }
                }

                return dictionary;
            }
        }

        throw new InvalidCastException($"Cannot convert {value.GetType()} to {targetType}.");
    }
}
