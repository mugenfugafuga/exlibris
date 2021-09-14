using ExcelDna.Integration;
using System;

namespace ExLibris.Core
{
    class ObjectHandle : IExcelObservable, IDisposable
    {
        private static readonly Action doNothing = () => { };

        public readonly string HandleKey;
        public readonly ObjectRepository objectRepository;
        private readonly Action runOnDisposing;

        public ObjectHandle(ObjectRepository objectRepository, object value) :
            this(value.GetType().FullName, objectRepository, value, doNothing)
        {
        }

        public ObjectHandle(ObjectRepository objectRepository, IDisposable disposable) :
            this(disposable.GetType().FullName, objectRepository, disposable)
        {
        }

        public ObjectHandle(string objectName, ObjectRepository objectRepository, object value) :
            this(objectName, objectRepository, value, doNothing)
        {
        }

        public ObjectHandle(string objectName, ObjectRepository objectRepository, IDisposable disposable) :
            this(objectName, objectRepository, disposable, () => disposable.Dispose())
        {
        }

        public ObjectHandle(string objectName, ObjectRepository objectRepository, object value, Action runOnDisposing)
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
