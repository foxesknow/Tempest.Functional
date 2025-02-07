using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;
using Tempest.Functional;
using Tempest.Functional.Threading;
using static Tempest.Functional.Keywords.Unit;

namespace Tests.Tempest.Functional.Threading
{
    [TestFixture]
    public class UnitTaskTests
    {
        [Test]
        public void CompletedTask()
        {
            Assert.That(UnitTask.CompletedTask, Is.Not.Null);
            Assert.That(UnitTask.CompletedTask.IsCompletedSuccessfully, Is.True);
            Assert.That(UnitTask.CompletedTask.Result, Is.EqualTo(unit));
        }

        [Test]
        public void FromException()
        {
            var e = new Exception();
            var task = UnitTask.FromException(e);

            Assert.That(task, Is.Not.Null);
            Assert.That(task.IsCompleted, Is.True);
            Assert.That(task.IsFaulted, Is.True);
            Assert.That(task.Exception, Is.Not.Null);
        }

        [Test]
        public void FromException_NoException()
        {
            Assert.Catch(() => UnitTask.FromException(null!));
        }

        [Test]
        public void FromCanceled()
        {
            var task = UnitTask.FromCanceled();
            Assert.That(task, Is.Not.Null);
            Assert.That(task.IsCompleted, Is.True);
            Assert.That(task.IsCanceled, Is.True);
        }

        [Test]
        public void FromCanceled_Token()
        {
            CancellationToken ct = new();

            var task = UnitTask.FromCanceled(ct);
            Assert.That(task, Is.Not.Null);
            Assert.That(task.IsCompleted, Is.True);
            Assert.That(task.IsCanceled, Is.True);
        }

        [Test]
        public void Task_Completed()
        {
            var task = Task.CompletedTask.ToUnitTask();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }

        [Test]
        public async Task Task_Delay()
        {
            var result = await Task.Delay(1000).ToUnitTask();
            Assert.That(result, Is.EqualTo(Unit.Value));
        }

        [Test]
        public void ValueTask_Completed()
        {
            var task = ValueTask.CompletedTask.ToUnitTask();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }

        [Test]
        public async ValueTask ValueTask_Delay()
        {
            var result = await Delay().ToUnitTask();
            Assert.That(result, Is.EqualTo(Unit.Value));

            static async ValueTask Delay()
            {
                await Task.Delay(1000);
            }
        }
    }
}
