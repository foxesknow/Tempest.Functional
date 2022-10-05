using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Tempest.Functional;

namespace Tests.Tempest.Functional
{
    [TestFixture]
    public partial class OptionExtensionTests
    {
        [Test]
        public void SelectMany_Linq()
        {
            Option<int> x = new(10);
            Option<int> y = new(20);

            var total = from a in x
                        from b in y
                        select a + b;

            Assert.That(total.Value(), Is.EqualTo(30));
        }

        [Test]
        public void SelectMany_Linq_None()
        {
            Option<int> x = new(10);
            Option<int> y = Option.None;

            var total = from a in x
                        from b in y
                        select a + b;

            Assert.That(total, Is.EqualTo(Option.None));
        }
    }
}
