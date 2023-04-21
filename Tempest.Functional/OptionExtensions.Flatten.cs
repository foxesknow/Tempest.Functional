using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tempest.Functional
{
    /// <summary>
    /// Useful option extension methods
    /// </summary>
    public static partial class OptionExtensions
    {
        /// <summary>
        /// Extracts an option from another option
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Option<T> Flatten<T>(in this Option<Option<T>> self)
        {
            return self.ValueOr(Option.None);
        }
    }
}
