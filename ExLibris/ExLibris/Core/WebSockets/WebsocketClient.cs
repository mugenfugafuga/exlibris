using SuperSocket.ClientEngine;
using System;
using System.Collections.Concurrent;
using WebSocket4Net;

namespace ExLibris.Core.WebSockets
{
    public class WebsocketClient : IDisposable
    {
        private ConcurrentDictionary<string, IWebsocketListener> listeners = new ConcurrentDictionary<string, IWebsocketListener>();
        public WebSocket WebSocket { get; }

        public WebsocketClient(string websocketUri)
        {
            this.WebSocket = new WebSocket(websocketUri);

            WebSocket.Opened += Opened;
            WebSocket.Closed += Closed;
            WebSocket.Error += Error;
            WebSocket.DataReceived += DataReceived;
            WebSocket.MessageReceived += MessageReceived;

            WebSocket.Open();
        }

        public void SendMessage(string message) => WebSocket.Send(message);

        public void AddListener(string name, IWebsocketListener listener) => listeners[name] = listener;

        public void RemoveListener(string name) => listeners.TryRemove(name, out var _);

        public void Dispose()
        {
            WebSocket.Close();
            WebSocket.Dispose();
        }

        private void Opened(object sender, EventArgs e)
        {
            foreach(var listener in listeners.Values)
            {
                listener.Opened(sender, e);
            }
        }

        private void Closed(object sender, EventArgs e)
        {
            foreach (var listener in listeners.Values)
            {
                listener.Closed(sender, e);
            }
        }

        private void Error(object sender, ErrorEventArgs e)
        {
            foreach (var listener in listeners.Values)
            {
                listener.Error(sender, e);
            }
        }

        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            foreach (var listener in listeners.Values)
            {
                listener.DataReceived(sender, e);
            }
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            foreach (var listener in listeners.Values)
            {
                listener.MessageReceived(sender, e);
            }
        }
    }
}
