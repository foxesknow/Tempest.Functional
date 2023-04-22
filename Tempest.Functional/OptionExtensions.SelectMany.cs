using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tempest.Functional
{
    public static partial class OptionExtensions
    {
        /// <summary>
        /// This SelectMany extension method enables Linq support
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="self"></param>
        /// <param name="optionSelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static Option<TResult> SelectMany<T, U, TResult>(in this Option<T> self, Func<T, Option<U>> optionSelector, Func<T, U, TResult> resultSelector)  
        {
            return self.Bind
            (
                (optionSelector, resultSelector), 
                static (t, state) => state.optionSelector(t).Select
                (
                    (state.resultSelector, t), 
                    static (u, state) => state.resultSelector(state.t, u)
                )
            );
        } 
    }
}
