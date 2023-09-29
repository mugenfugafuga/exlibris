using ExcelDna.Integration;
using System;
using System.Net.Http.Headers;

namespace Exlibris.Functions.WebAPI
{
    partial class WebAPIFunctions
    {
        [ExcelFunction(
            Category = Category,
            Name = Category + ".Authentication",
            Description = "create Authentication Header",
            IsHidden = true)]
        public static object Authentication(
            [ExcelArgument(AllowReference = true, Description = "schema. 0 : basic, 1 : Bearer, or string : other schema")] object schema,
            [ExcelArgument(AllowReference = true, Description = "certification. if basic authentication, the string obtained by Base64 encoding '[account]:[password]', if Bearer, AccessToken")] object certification,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
            => ExlibrisAddin.GetFunctionSupport().ErrorValueIfThrown(support =>
            {
                var schemaVal = support.NotDateTime(schema, nameof(schema)).ShouldBeScalar();
                var certificationVal = support.NotDateTime(certification, nameof(certification)).ShouldBeScalar().GetValueOrThrow<string>();

                return support.ObserveObjectRegistration(() =>
                {
                    if (schemaVal.TryGetValue<int>(out var schm))
                    {
                        switch (schm)
                        {
                            case 0: return new AuthenticationHeaderValue("Basic", certificationVal);
                            case 1: return new AuthenticationHeaderValue("Bearer", certificationVal);
                            default: throw new ArgumentException($"{nameof(schema)} is invalid argument. value : {schm}");
                        }
                    }

                    return new AuthenticationHeaderValue(schemaVal.GetValueOrThrow<string>(), certificationVal);
                }, schema, certification, identifier);
            });
    }
}