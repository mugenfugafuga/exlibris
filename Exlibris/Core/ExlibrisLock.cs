using System.Threading;
using System;

namespace Exlibris.Core
{
    public class ExlibrisLock
    {
        private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public IDisposable GetReadLock() => new ReaderLocker(locker);

        public IDisposable GetWriteLock() => new WriterLocker(locker);

        private class ReaderLocker : AbstractDisposable
        {
            private readonly ReaderWriterLockSlim locker;

            public ReaderLocker(ReaderWriterLockSlim locker)
            {
                this.locker = locker;
                locker.EnterReadLock();
            }

            public override void OnDisposing() => locker.ExitReadLock();
        }

        private class WriterLocker : AbstractDisposable
        {
            private readonly ReaderWriterLockSlim locker;

            public WriterLocker(ReaderWriterLockSlim locker)
            {
                this.locker = locker;
                locker.EnterWriteLock();
            }

            public override void OnDisposing() => locker.ExitWriteLock();
        }
    }
}