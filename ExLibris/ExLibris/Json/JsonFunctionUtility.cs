using ExcelDna.Integration;
using ExLibris.Core;
using System;

namespace ExLibris.Json
{
    internal class JsonFunctionUtility
    {
        private const string jsonObjectName = "JsonObject";

        public static IExcelObservable NewObservableJsonObjectHandle(ObjectRepository objectRepository, Func<object> func)
            => ExLibrisUtility.NewObservableObjectRegistrationHandle(NewJsonObjectHandle(objectRepository, func));

        public static IExcelObservable NewObservableJsonObjectHandleAsync(ObjectRepository objectRepository, Func<object> func)
            => ExLibrisUtility.NewObservableObjectRegistrationHandleAsync(NewJsonObjectHandle(objectRepository, func));

        public static ObjectRegistrationHandle<object> NewJsonObjectHandle(ObjectRepository objectRepository, Func<object> func)
            => new ObjectRegistrationHandle<object>(jsonObjectName, objectRepository, func);

        public static object ObserveJsonObject(
        string callerFunctionName,
        ObjectRepository objectRepository,
        Func<object> jsonObjectFunc,
        params object[] paramObjects)
        => ExLibrisUtility.ExcelObserveObjectRegistration(
            callerFunctionName,
            jsonObjectName,
            objectRepository,
            jsonObjectFunc,
            paramObjects);

        public static object ObserveJsonObjectAsync(
        string callerFunctionName,
        ObjectRepository objectRepository,
        Func<object> jsonObjectFunc,
        params object[] paramObjects)
        => ExLibrisUtility.ExcelObserveObjectRegistrationAsync(
            callerFunctionName,
            jsonObjectName,
            objectRepository,
            jsonObjectFunc,
            paramObjects);

    }
}
