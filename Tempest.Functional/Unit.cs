using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tempest.Functional
{
    /// <summary>
    /// A unit type
    /// </summary>
    public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
    {
        /// <summary>
        /// Always returns 0
        /// </summary>
        /// <param name="other"></param>
        /// <returns>0</returns>
        public int CompareTo(Unit other)
        {
            return 0;
        }

        /// <summary>
        /// Always returns true
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Unit other)
        {
            return true;
        }

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Unit;
        }        

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "()";
        }
    }
}
