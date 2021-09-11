using ExLibris.Core;

namespace ExLibris
{
    public class ExLibrisContext
    {
        public static ExLibrisContext DefaultContext = new ExLibrisContext();

        public ObjectRepository ObjectRepository { get; set; } = new ObjectRepository();
    }
}
