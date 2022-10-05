namespace Tempest.Functional.Impl
{
    /// <summary>
    /// Helps with method resolution
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class RequireClass<T> where T : class 
    { 
        private RequireClass()
        {
        }
    }
}
