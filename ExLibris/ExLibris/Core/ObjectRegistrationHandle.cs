using ExcelDna.Integration;
using System;

namespace ExLibris.Core
{
    class ObjectRegistrationHandle : IExcelObservable, IDisposable
    {
        private static readonly Action doNothing = () => { };

        public readonly string HandleKey;
        public readonly ObjectRepository objectRepository;
        private readonly Action runOnDisposing;

        public ObjectRegistrationHandle(ObjectRepository objectRepository, object value) :
            this(value.GetType().FullName, objectRepository, value, doNothing)
        {
        }

        public ObjectRegistrationHandle(ObjectRepository objectRepository, IDisposable disposable) :
            this(disposable.GetType().FullName, objectRepository, disposable)
        {
        }

        public ObjectRegistrationHandle(string objectName, ObjectRepository objectRepository, object value) :
            this(objectName, objectRepository, value, doNothing)
        {
        }

        public ObjectRegistrationHandle(string objectName, ObjectRepository objectRepository, IDisposable disposable) :
            this(objectName, objectRepository, disposable, () => disposable.Dispose())
        {
        }

        public ObjectRegistrationHandle(string objectName, ObjectRepository objectRepository, object value, Action runOnDisposing)
        {
            HandleKey = $"{objectName}:{Guid.NewGuid().ToString().ToUpper()}";
            this.objectRepository = objectRepository;
            this.runOnDisposing = runOnDisposing;

            objectRepository.RegisterObject(HandleKey, value);
        }

        public virtual void Dispose()
        {
            objectRepository.Remove(HandleKey);
            runOnDisposing();
        }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            observer.OnNext(HandleKey);
            return this;
        }
    }
}
