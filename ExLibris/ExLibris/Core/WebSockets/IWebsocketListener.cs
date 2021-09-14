using SuperSocket.ClientEngine;
using System;
using WebSocket4Net;

namespace ExLibris.Core.WebSockets
{
    public interface IWebsocketListener
    {
        void Opened(object sender, EventArgs e);
        void Closed(object sender, EventArgs e);
        void Error(object sender, ErrorEventArgs e);
        void DataReceived(object sender, DataReceivedEventArgs e);
        void MessageReceived(object sender, MessageReceivedEventArgs e);
    }
}
