using ExcelDna.Integration;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Concurrent;
using WebSocket4Net;

namespace ExLibris.Core.WebSockets
{
    class WebSocketMessageReveiverHandle : IExcelObservable, IDisposable, IWebsocketListener
    {
        private readonly ConcurrentBag<IExcelObserver> observers = new ConcurrentBag<IExcelObserver>();
        private readonly string name = $"Listener:{Guid.NewGuid()}";

        private readonly WebsocketClient client;

        public WebSocketMessageReveiverHandle(WebsocketClient client)
        {
            this.client = client;
            client.AddListener(name, this);
        }

        public void Dispose()
        {
            client.RemoveListener(name);
        }


        public void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            foreach(var obs in observers)
            {
                obs.OnNext(e.Message);
            }
        }


        public IDisposable Subscribe(IExcelObserver observer)
        {
            observers.Add(observer);
            return this;
        }

        public void Opened(object sender, EventArgs e) { }
        public void Closed(object sender, EventArgs e) { }
        public void DataReceived(object sender, DataReceivedEventArgs e) { }
        public void Error(object sender, ErrorEventArgs e) { }
    }
}
