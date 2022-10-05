namespace Tempest.Functional.Impl
{
    /// <summary>
    /// Helps with method resolution
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class RequireStruct<T> where T : struct 
    { 
        private RequireStruct()
        {
        }
    }
}
