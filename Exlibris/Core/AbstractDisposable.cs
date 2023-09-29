using System;

namespace Exlibris.Core
{
    public abstract class AbstractDisposable : IDisposable
    {
        private bool disposed = false;

        ~AbstractDisposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    OnDisposing();
                }

                disposed = true;
            }
        }

        public abstract void OnDisposing();
    }
}