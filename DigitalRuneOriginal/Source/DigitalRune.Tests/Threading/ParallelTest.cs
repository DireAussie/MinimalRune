using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;


using System.Threading;



namespace DigitalRune.Threading.Tests
{
  [TestFixture]
  public class ParallelTest
  {

    private static void Sleep(int millisecondsTimeout)
    {
      System.Threading.Tasks.Task.Delay(millisecondsTimeout).Wait();
    }
#else
    private static void Sleep(int millisecondsTimeout)
    {
      Thread.Sleep(millisecondsTimeout);
    }



    private static void AssertException(string expectedMessage, Exception exception)
    {
      // When using Task Parallel Library (TPL) in Windows Store build: 
      // We either get
      //   TaskException -> Exception
      // or
      //   TaskException -> AggregateException -> Exception
      // Both result are correct.

      // AssertException checks the message of the inner-most exception.
      Exception innerException = exception;
      var innerExceptions = GetInnerExceptions(exception);
      while (innerExceptions != null)
      {
        innerException = innerExceptions.FirstOrDefault();
        innerExceptions = GetInnerExceptions(innerException);
      }

      Assert.IsNotNull(innerException);
      Assert.AreEqual(expectedMessage, innerException.Message);
    }


    private static IEnumerable<Exception> GetInnerExceptions(Exception exception)
    {
      var taskException = exception as TaskException;
      if (taskException != null)
        return taskException.InnerExceptions;


      var aggregateException = exception as AggregateException;
      if (aggregateException != null)
        return aggregateException.InnerExceptions;


      return null;
    }

      
    [Test]
    public void NestedParallelForLoops()
    {
      Parallel.For(0, 100, i =>
      {
        Parallel.For(0, 100, j =>
        {
          Parallel.For(0, 100, k =>
          {

          });
        });
      });
    }



    [Test]
    public void ProcessorAffinityAndNumberOfThreads()
    {
      // Configure Parallel to use the hardware threads 3, 4, 5 on the Xbox 360.
      // (Note: Setting the processor affinity has no effect on Windows.)
      Parallel.ProcessorAffinity = new[] { 3, 4, 5 };

      // Create task scheduler that uses 3 worker threads.
      Parallel.Scheduler = new WorkStealingScheduler(3);

      List<int> threadIds = new List<int>();

      Parallel.For(0, 100, i =>
      {
        lock (threadIds)
        {
          if (!threadIds.Contains(Thread.CurrentThread.ManagedThreadId))
            threadIds.Add(Thread.CurrentThread.ManagedThreadId);
        }
        Sleep(1);
      });


      // Security issues on Windows Phone: Cannot access Environment.ProcessorCount.
      var processorCount = 1;
#else
      var processorCount = Environment.ProcessorCount;


      if (processorCount == 4)
      {
        // Assert only if on quad core. (Result may be wrong on a system with less CPU cores.)
        Assert.AreEqual(3, threadIds.Except(new[] { Thread.CurrentThread.ManagedThreadId }).Count());
      }
    }


    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ParallelShouldThrowWhenProcessorAffinityIsNull()
    {
      Parallel.ProcessorAffinity = null;
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ParallelShouldThrowWhenProcessorAffinityIsEmpty()
    {
      Parallel.ProcessorAffinity = new int[0];
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ParallelShouldThrowWhenProcessorAffinityIsInvalid()
    {
      Parallel.ProcessorAffinity = new[] { -1 };
    }


    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ParallelShouldThrowWhenSchedulerIsNull()
    {
      Parallel.Scheduler = null;
    }



    [Test]
    public void TaskException()
    {
      Task task = Parallel.Start(() => { throw new Exception("123"); });
      Sleep(100);
      Assert.IsTrue(task.IsComplete);
      Assert.IsNotNull(task.Exceptions);
      Assert.AreEqual(1, task.Exceptions.Length);
      AssertException("123", task.Exceptions[0]);
    }


    [Test]
    [ExpectedException(typeof(TaskException))]
    public void WaitShouldThrowOnException()
    {
      Task task = Parallel.Start(() => { throw new Exception("123"); });
      Sleep(100);
      task.Wait();
    }


    [Test]
    public void ShouldCatchExceptionInNestedTask()
    {
      bool exceptionCaught = false;
      try
      {
        Task task = Parallel.Start(() =>
                    {
                      Task t = Parallel.Start(() => { throw new Exception("Nested Exception"); });
                      // Do not wait for task t!
                      // The exception of t should be caught in task.Wait() below.
                    });
        task.Wait();
      }
      catch (TaskException exception)
      {
        exceptionCaught = true;
        AssertException("Nested Exception", exception);
      }

      Assert.IsTrue(exceptionCaught);
    }


    [Test]
    [ExpectedException(typeof(TaskException))]
    public void BackgroundTaskShouldCatchExceptions()
    {
      Task task = Parallel.StartBackground(() => { throw new Exception("123"); });
      Sleep(100);
      task.Wait();
    }


    [Test]
    public void BackgroundTaskShouldCatchExceptionInNestedTask()
    {
      bool exceptionCaught = false;
      try
      {
        Task task = Parallel.StartBackground(() =>
                    {
                      Task t = Parallel.Start(() => { throw new Exception("Nested Exception"); });
                      // Do not wait for task t!
                      // The exception of t should be caught in task.Wait() below.
                    });
        task.Wait();
      }
      catch (TaskException exception)
      {
        exceptionCaught = true;
        AssertException("Nested Exception", exception);
      }

      Assert.IsTrue(exceptionCaught);
    }


    class TestEnumerable : IEnumerable<int>
    {
      public struct Enumerator : IEnumerator<int>
      {
        private TestEnumerable _testEnumerable;
        private int _index;
        private int _current;

        public int Current { get { return _current; } }
        object IEnumerator.Current { get { return _current; } }

        public Enumerator(TestEnumerable testEnumerable)
        {
          _testEnumerable = testEnumerable;
          _index = 0;
          _current = 0;
        }

        public void Dispose()
        {
          _testEnumerable.EnumeratorDisposed = true;
          _index = -1;
        }

        public bool MoveNext()
        {
          if (_index == -1)
            return false;

          if (_index < _testEnumerable.Result.Length)
          {
            _current = _index;
            _index++;
            return true;
          }

          _index = -1;
          return false;
        }

        public void Reset()
        {
          _index = 0;
        }
      }

      public bool EnumeratorDisposed = false;
      public int[] Result = new int[1000];

      public Enumerator GetEnumerator()
      {
        return new Enumerator(this);
      }

      IEnumerator<int> IEnumerable<int>.GetEnumerator()
      {
        return GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return GetEnumerator();
      }
    }


    [Test]
    public void ParallelForEach()
    {
      var testEnumerable = new TestEnumerable();


      Parallel.ForEach(testEnumerable, i => testEnumerable.Result[i] = Environment.CurrentManagedThreadId);
#else
      Parallel.ForEach(testEnumerable, i => testEnumerable.Result[i] = Thread.CurrentThread.ManagedThreadId);


      Assert.IsTrue(testEnumerable.Result.All(n => n != 0));
      Assert.IsTrue(testEnumerable.EnumeratorDisposed);
    }
  }
}
