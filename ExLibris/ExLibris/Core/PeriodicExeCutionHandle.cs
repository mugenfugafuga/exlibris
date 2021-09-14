using ExcelDna.Integration;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ExLibris.Core
{
    public class PeriodicExeCutionHandle : IExcelObservable, IDisposable
    {
        private readonly ConcurrentBag<IExcelObserver> observers = new ConcurrentBag<IExcelObserver>();
        private readonly Func<object> func;
        private readonly Timer timer;
        private object current = null;
        private object locker = new object();

        public PeriodicExeCutionHandle(Func<object> func, int periodMilliSec)
        {
            this.func = func;
            this.timer = new Timer(TimerCallback, null, 0, periodMilliSec);
        }

        public void Dispose()
        {
            timer.Dispose();
        }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            observers.Add(observer);
            return this;
        }

        private void TimerCallback(object _)
        {
            var o = func();

            lock (locker)
            {
                if (current != null && current.Equals(o))
                {
                    return;
                }

                current = o;
            }

            foreach (var obs in observers)
            {
                obs.OnNext(o);
            }
        }
    }
}
