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
        public void OrElse()
        {
            Option<int> x = 10;
            var y = x.OrElse(20);
            Assert.That(y.Value(), Is.EqualTo(10));
        }

        [Test]
        public void OrElse_None()
        {
            Option<int> x = default;
            var y = x.OrElse(20);
            Assert.That(y.Value(), Is.EqualTo(20));
        }

        [Test]
        public void OrElse_None_None()
        {
            Option<int> x = default;
            var y = x.OrElse(Option.None);
            Assert.That(y, Is.EqualTo(Option.None));
        }

        [Test]
        public void OrElse_Func()
        {
            Option<int> x = default;
            var y = x.OrElse(static () => 20);
            Assert.That(y.Value(), Is.EqualTo(20));
        }

        [Test]
        public void OrElse_Func_NotCalled()
        {
            bool called = false;

            Option<int> x = 10;
            var y = x.OrElse(() => {called = true; return 20;});
            Assert.That(y.Value(), Is.EqualTo(10));
            Assert.That(called, Is.False);
        }

        [Test]
        public void OrElse_Func_State()
        {
            var answer = 20;

            Option<int> x = default;
            var y = x.OrElse(answer, static state => state);
            Assert.That(y.Value(), Is.EqualTo(20));
        }

        [Test]
        public void OrElse_Func_State_NotCalled()
        {
            var answer = 20;
            bool called = false;

            Option<int> x = default;
            var y = x.OrElse(answer, state => {called = true; return state;});
            Assert.That(y.Value(), Is.EqualTo(20));
            Assert.That(called, Is.True);
        }
    }
}
