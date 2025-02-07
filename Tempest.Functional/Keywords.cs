using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tempest.Functional
{
    /// <summary>
    /// Allows you to treat certain types as keywords via the <![CDATA[using static]]> syntax
    /// </summary>
    public static class Keywords
    {
        /// <summary>
        /// Introduces a none "keyword"
        /// </summary>
        /// <remarks>To bring it into scope use <![CDATA[using static Tempest.Functional.Keywords.None]]> </remarks>
        public static class None
        {
            /// <summary>
            /// none
            /// </summary>
            public static readonly Tempest.Functional.None none = new();
        }

        /// <summary>
        /// Introduces a unit "keyword"
        /// </summary>
        /// <remarks>To bring it into scope use <![CDATA[using static Tempest.Functional.Keywords.Unit]]> </remarks>
        public static class Unit
        {
            /// <summary>
            /// unit
            /// </summary>
            public static readonly Tempest.Functional.Unit unit = new();
        }
    }
}
