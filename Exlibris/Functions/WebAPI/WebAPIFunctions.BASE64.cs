using ExcelDna.Integration;
using Exlibris.Core.WebAPI;

namespace Exlibris.Functions.WebAPI
{
    partial class WebAPIFunctions
    {
        [ExcelFunction(
        Category = Category,
            Name = Category + ".BASE64",
        Description = "BASE64")]
        public static object BASE64(
            [ExcelArgument(AllowReference = true, Description = "account")] object account,
            [ExcelArgument(AllowReference = true, Description = "password")] object password,
            [ExcelArgument(AllowReference = true, Description = "setting the argument to ‘true’ loads the text file as an object in the memory, while setting it to ‘false’ displays the text on Excel.")] object asObject,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
            => ExlibrisAddin.GetFunctionSupport().ErrorValueIfThrown(support =>
            {
                var accountVal = support.NotDateTime(account, nameof(account)).ShouldBeScalar().GetValueOrThrow<string>();
                var passwordVal = support.NotDateTime(password, nameof(password)).ShouldBeScalar().GetValueOrThrow<string>();

                if (support.NotDateTime(asObject, nameof(asObject)).ShouldBeScalar().GetValueOrDefault(true))
                {
                    return support.ObserveObjectRegistration(() =>
                    {
                        return WebAPIUtil.ToBASE64(accountVal, passwordVal);
                    }, account, password, asObject, identifier);
                }
                else
                {
                    return WebAPIUtil.ToBASE64(accountVal, passwordVal);
                }
            });
    }
}