using ExLibris.Core;

namespace ExLibris
{
    public class ExLibrisContext
    {
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
    }
}
