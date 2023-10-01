using System.Threading.Tasks;

namespace Exlibris.Core
{
    public interface IReceiver
    {
        Task<Message> ReceiveAsync();
    }
}
