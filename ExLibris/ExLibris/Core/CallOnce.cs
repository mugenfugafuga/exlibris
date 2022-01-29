using System;
using System.Threading;

namespace ExLibris.Core
{
    public class CallOnce
    {
        private const long NotYet = 0;
        private readonly Action action;
        private readonly object locker = new object();
        private long flag = NotYet;


        public CallOnce(Action action)
        {
            this.action = action;
        }

        public void Invoke()
        {
            if (Interlocked.Read(ref flag) != NotYet)
            {
                return;
            }

            lock(locker)
            {
                if (Interlocked.Read(ref flag) != NotYet)
                {
                    return;
                }

                action();
                Interlocked.Increment(ref flag);
            }
        }
    }
}
