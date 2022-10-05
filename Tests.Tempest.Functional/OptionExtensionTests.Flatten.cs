using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Tempest.Functional;

namespace Tests.Tempest.Functional
{
    public partial class OptionExtensionTests
    {
        [Test]
        public void Flatten()
        {
            var inner = Option.Some("Hello");
            var outer = Option.Some(inner);
            Assert.That(outer.Value(), Is.EqualTo(inner));

            var flattened = outer.Flatten();
            Assert.That(flattened, Is.EqualTo(inner));
        }
    }
}
