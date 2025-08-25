namespace backend.Exceptions
{
    /// <summary>
    /// Exception thrown when an entity cannot be found in the data store.
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string entity, object? key = null)
            : base(key is null
                ? $" No {entity} records found."
                : $"{entity} with key '{key}' was not found.")
        {
        }
    }
}
