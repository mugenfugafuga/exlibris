namespace Exlibris.Core
{
    public class Atomic<T>
    {
        private readonly ExlibrisLock locker = new ExlibrisLock();
        private T value;

        public Atomic(T value)
        {
            this.value = value;
        }

        public T Value
        {
            get
            {
                using (var _ = locker.GetReadLock())
                {
                    return value;
                }
            }
            set
            {
                using (var _ = locker.GetWriteLock())
                {
                    this.value = value;
                }
            }
        }
    }
}