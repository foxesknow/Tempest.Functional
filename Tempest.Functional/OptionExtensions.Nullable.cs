using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tempest.Functional.Impl;

namespace Tempest.Functional
{
    public static partial class OptionExtensions
    {
        /// <summary>
        /// Converts an option to a nullable value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="_">Ensure the method is called only for value types. Just accept the default value</param>
        /// <returns></returns>
        public static T? ToNullable<T>(in this Option<T> self, RequireStruct<T>? _ = null) where T : struct
        {
            if(self.IsSome)
            {
                return self.Value();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts an option to a nullable reference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="_">Ensure the method is called only for reference types. Just accept the default value</param>
        /// <returns></returns>
        public static T? ToNullable<T>(in this Option<T> self, RequireClass<T>? _ = null) where T : class
        {
            if(self.IsSome)
            {
                return self.Value();
            }
            else
            {
                return null;
            }
        }
    }
}
