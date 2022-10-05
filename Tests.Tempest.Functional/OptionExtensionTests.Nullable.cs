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
        public void ToNullable_Value()
        {
            Option<int> age = 10;
            var nullableAge = age.ToNullable();
            Assert.That(nullableAge.HasValue, Is.True);
            Assert.That(nullableAge!.Value, Is.EqualTo(10));
        }

        [Test]
        public void ToNullable_Value_Null()
        {
            Option<int> age = Option.None;
            var nullableAge = age.ToNullable();
            Assert.That(nullableAge.HasValue, Is.False);
        }

        [Test]
        public void ToNullable_Reference()
        {
            Option<string> name = "Robert";
            var nullableName = name.ToNullable();
            Assert.That(nullableName, Is.Not.Null);
            Assert.That(nullableName!, Is.EqualTo("Robert"));
        }

        [Test]
        public void ToNullable_Reference_Null()
        {
            Option<string> name = Option.None;
            var nullableName = name.ToNullable();
            Assert.That(nullableName, Is.Null);
        }
    }
}
