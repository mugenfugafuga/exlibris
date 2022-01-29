using System;

namespace ExLibris.Core
{
    public class ObjectRegistrationHandle<T> : IDisposable
    {
        public string HandleKey { get; }
        public readonly ObjectRepository objectRepository;

        public T Value { get; private set; }

        private readonly Func<T> objectFunc;

        public ObjectRegistrationHandle(ObjectRepository objectRepository, Func<T> objectFunc) :
            this(typeof(T).FullName, objectRepository, objectFunc)
        {
        }

        public ObjectRegistrationHandle(string objectName, ObjectRepository objectRepository, Func<T> objectFunc)
        {
            HandleKey = $"{objectName}:{Guid.NewGuid().ToString().ToUpper()}";
            this.objectRepository = objectRepository;
            this.objectFunc = objectFunc;
        }

        public virtual void Dispose()
        {
            objectRepository.Remove(HandleKey);

            if (Value != null && Value is IDisposable val)
            {
                val.Dispose();
            }
        }

        public void CallRegistration()
        {
            Value = objectFunc();
            objectRepository.RegisterObject(HandleKey, Value);
        }
    }
}
