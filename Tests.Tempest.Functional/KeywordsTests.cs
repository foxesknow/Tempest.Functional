using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Tempest.Functional;
using static Tempest.Functional.Keywords.None;
using static Tempest.Functional.Keywords.Unit;

namespace Tests.Tempest.Functional
{
    [TestFixture]
    public class KeywordsTests
    {
        [Test]
        public void None_CompareWithOption()
        {
            Assert.That(Option.None.Equals(none), Is.True);
        }

        [Test]
        public void None_Compare()
        {
            Option<int> missing = none;
            Assert.That(missing, Is.EqualTo(none));
        }

        [Test]
        public void None_Compare_Some()
        {
            Option<int> missing = 10;
            Assert.That(missing, Is.Not.EqualTo(none));
        }

        [Test]
        public void Unit()
        {
            Unit x = new();
            Assert.That(x, Is.EqualTo(unit));
        }
    }
}
