using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BattleRoyale_Server
{

    abstract class BaseThread
    {
        private Thread _thread;
        protected BaseThread()
        {
            _thread = new Thread(new ThreadStart(this.RunThread));
        }

        // Thread methods / properties
        public void Start() => _thread.Start();
        public void Join() => _thread.Join();
        public bool IsAlive => _thread.IsAlive;

        // Override in base class
        public abstract void RunThread();
    }
    class ClientThread : BaseThread
    {
        private Received socket;
        private int timeout;
        private int clientId;
        bool connected = true;
        public ClientThread(Received socket, int clientId) : base()
        {
            this.socket = socket;
            this.timeout = 0;
            this.clientId = clientId;
            
            Reply("Connected!");
            checkAliveListener();
        }

        public override void RunThread()
        {
          
        }

        public void Receive(string message)
        {
            switch (message)
            {
                case "alive":
                    timeout = 0;
                    Console.WriteLine("Alive from " + getSender());
                    break;
            }
        }

        private void Reply(String message)
        {
            //Program.server.Reply("copy " + socket.Message, socket.Sender);
            Program.server.Reply(message, socket.Sender);
        }

        private void checkAliveListener()
        {
            Task.Factory.StartNew(async () =>
            {
                while (connected)
                {
                    if (timeout >= 10)
                    {
                        connected = false;
                        Console.WriteLine("[TIMEOUT] " + socket.Sender.ToString());
                        Program.removeClient(clientId);
                        return;
                    }
                    Reply("alive");
                    timeout += 1;
                    Thread.Sleep(1000);
                }
            });
        }

        public int getClientId()
        {
            return clientId;
        }

        public System.Net.IPEndPoint getSender()
        {
            return socket.Sender;
        }
    }
}
