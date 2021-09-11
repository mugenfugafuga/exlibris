using ExcelDna.Integration;
using System;

namespace ExLibris.Core
{
    class ObjectHandle : IExcelObservable, IDisposable
    {
        public readonly string HandleKey;
        public readonly ObjectRepository objectRepository;

        public ObjectHandle(ObjectRepository objectRepository, object value) :
            this(value.GetType().FullName, objectRepository, value)
        {
        }

        public ObjectHandle(string objectName, ObjectRepository objectRepository, object value)
        {
            HandleKey = $"{objectName}:{Guid.NewGuid().ToString().ToUpper()}";
            this.objectRepository = objectRepository;

            objectRepository.RegisterObject(HandleKey, value);
        }

        public virtual void Dispose()
        {
            objectRepository.Remove(HandleKey);
        }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            observer.OnNext(HandleKey);
            return this;
        }
    }
}
