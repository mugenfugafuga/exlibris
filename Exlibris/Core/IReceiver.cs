using System.Threading.Tasks;

namespace Exlibris.Core
{
    interface IReceiver
    {
        Task<Message> ReceiveAsync();
    }
}
