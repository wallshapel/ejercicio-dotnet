using System.Collections.Concurrent;
using System.Reflection;

namespace backend.Common.Mapping
{
    public class ReflectionObjectMapper : IObjectMapper
    {
        // Cache matched property pairs by (sourceType, destType)
        private static readonly ConcurrentDictionary<(Type src, Type dst), List<(PropertyInfo src, PropertyInfo dst)>> _mapCache = new();

        public TDest Map<TSource, TDest>(TSource source, params string[] ignoreDestProps) where TDest : new()
        {
            if (source is null) return new TDest();
            var dest = new TDest();
            MapInto(source, dest, ignoreNulls: false, ignoreDestProps);
            return dest;
        }

        public void MapInto<TSource, TDest>(TSource source, TDest dest, bool ignoreNulls = false, params string[] ignoreDestProps)
        {
            if (source is null || dest is null) return;

            var srcType = typeof(TSource);
            var dstType = typeof(TDest);

            var pairs = _mapCache.GetOrAdd((srcType, dstType), key =>
            {
                var srcProps = key.src.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead);
                var dstProps = key.dst.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite);

                var dictSrc = srcProps.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

                var list = new List<(PropertyInfo src, PropertyInfo dst)>();
                foreach (var d in dstProps)
                {
                    if (dictSrc.TryGetValue(d.Name, out var s))
                        list.Add((s, d));
                }
                return list;
            });

            var ignore = new HashSet<string>(ignoreDestProps ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);

            foreach (var (sProp, dProp) in pairs)
            {
                if (ignore.Contains(dProp.Name)) continue;

                var value = sProp.GetValue(source);

                if (ignoreNulls && value is null) continue;

                if (value is null)
                {
                    // Allow assigning null to reference or nullable types
                    if (!dProp.PropertyType.IsValueType || Nullable.GetUnderlyingType(dProp.PropertyType) != null)
                        dProp.SetValue(dest, null);
                    continue;
                }

                var srcTypeNonNull = Nullable.GetUnderlyingType(sProp.PropertyType) ?? sProp.PropertyType;
                var dstTypeNonNull = Nullable.GetUnderlyingType(dProp.PropertyType) ?? dProp.PropertyType;

                object? converted = null;

                if (dstTypeNonNull.IsAssignableFrom(srcTypeNonNull))
                    converted = value;
                else
                {
                    try
                    {
                        if (dstTypeNonNull.IsEnum)
                        {
                            if (value is string str)
                                converted = Enum.Parse(dstTypeNonNull, str, ignoreCase: true);
                            else
                                converted = Enum.ToObject(dstTypeNonNull, Convert.ChangeType(value, Enum.GetUnderlyingType(dstTypeNonNull)));
                        }
                        else                       
                            converted = Convert.ChangeType(value, dstTypeNonNull);                       
                    }
                    catch
                    {
                        // Skip non-convertible properties to keep mapping resilient
                        continue;
                    }
                }

                dProp.SetValue(dest, converted);
            }
        }
    }
}
