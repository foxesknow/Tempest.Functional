using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Tempest.Functional.DI;

namespace Tests.Tempest.Functional.DI
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void Register()
        {
            var container = Container.Empty
                            .Register<Foo, Foo>(null)
                            .Register<Bar, Bar>(null)
                            .Register<Person, Person>(null)
                            .Register<Address, Address>(null);

            Assert.That(container, Is.Not.Null);

            Assert.That(container.TryCreate(typeof(Person), out var instance), Is.True);
            Assert.That(instance, Is.Not.Null);
        }

        class Foo
        {
        }
        
        class Bar
        {
        }

        class Person
        {
            public Person(Foo foo, Bar bar, Person outer)
            {
            }
        }

        class Address
        {
        }
    }
}
