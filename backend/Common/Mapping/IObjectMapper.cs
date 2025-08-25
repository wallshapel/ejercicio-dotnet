// Namespace aligned to project; keep code-only comments in English.
namespace backend.Common.Mapping
{
    public interface IObjectMapper
    {
        // Creates a new TDest and copies matching properties from source.
        TDest Map<TSource, TDest>(TSource source, params string[] ignoreDestProps)
            where TDest : new();

        // Copies properties from source into an existing dest instance.
        // ignoreNulls=true avoids overwriting with null (useful for PATCH).
        // ignoreDestProps avoids touching specific destination fields (e.g., PKs).
        void MapInto<TSource, TDest>(TSource source, TDest dest, bool ignoreNulls = false, params string[] ignoreDestProps);
    }
}
