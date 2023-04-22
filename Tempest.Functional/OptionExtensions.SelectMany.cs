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
        /// <param name="k"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Option<TResult> SelectMany<T, U, TResult>(in this Option<T> self, Func<T, Option<U>> k, Func<T, U, TResult> s)  
        {
            return self.Bind((k, s), static (t, state) => state.k(t).Select((state.s, t), static (u, state) => state.s(state.t, u)));
        } 
    }
}
