using System.Net;

namespace ExLibris.Core
{
    public class ExLibrisContext
    {
        public static CallOnce SecurityProtocolUpdateFunction = new CallOnce(()
            => ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);

        public static ExLibrisContext DefaultContext = new ExLibrisContext();

        public ObjectRepository ObjectRepository { get; set; } = new ObjectRepository();

        public ExLibrisConfiguration DefaultExLibrisConfiguration { get; set; } = new ExLibrisConfiguration();

        public ExLibrisConfiguration GetConfiguration(string confHandleKey)
        {
            if(!string.IsNullOrEmpty(confHandleKey) && ObjectRepository.TryGetObject<ExLibrisConfiguration>(confHandleKey, out var conf))
            {
                return conf;
            }

            return DefaultExLibrisConfiguration;
        }

        public ExcelFunctionCallSupport GetFunctionCallSupport()
            => new ExcelFunctionCallSupport(ObjectRepository, DefaultExLibrisConfiguration);

        public ExcelFunctionCallSupport GetFunctionCallSupport(string confHandleKey)
            => new ExcelFunctionCallSupport(ObjectRepository, GetConfiguration(confHandleKey));
    }
}
