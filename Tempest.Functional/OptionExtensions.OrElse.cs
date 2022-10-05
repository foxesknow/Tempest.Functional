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
        /// Returns self if it is some, otherwise returns other
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static Option<T> OrElse<T>(in this Option<T> self, in Option<T> other) where T : notnull
        {
            return self.IsSome ? self : other;
        }

        /// <summary>
        /// Returns self if it is some, otherwise calls a function to get an alternative value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Option<T> OrElse<T>(in this Option<T> self, Func<Option<T>> function) where T : notnull
        {
            if(function is null) throw new ArgumentNullException(nameof(function));

            return self.IsSome ? self : function();
        }

        /// <summary>
        /// Returns self if it is some, otherwise calls a function to get an alternative value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TState"></typeparam>
        /// <param name="self"></param>
        /// <param name="state">Additional state to pass to the function</param>
        /// <param name="function"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Option<T> OrElse<T, TState>(in this Option<T> self, TState state, Func<TState, Option<T>> function) where T : notnull
        {
            if(function is null) throw new ArgumentNullException(nameof(function));

            return self.IsSome ? self : function(state);
        }
    }
}
