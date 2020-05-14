using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
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
        private UdpClient udpClient;
        IPEndPoint address;
        private IPAddress clientIp;
        private int port;
        private int timeout;
        private int clientId;
        bool connected = true;
        public ClientThread(IPEndPoint address, int clientId) : base()
        {
            Console.WriteLine("Setup client...");
            this.address = address;
            this.udpClient = new UdpClient();
            this.timeout = 0;
            this.clientId = clientId;
            Console.WriteLine("Sending ID...");

            Send(clientId.ToString());
            checkAliveListener();
        }

        public override void RunThread()
        {
          
        }

        public void Receive(string[] message)
        {
            switch (message[1])
            {
                case "alive":
                    timeout = 0;
                    Console.WriteLine("Alive from " + address);
                    break;
                case "newplayer":
                    Console.WriteLine("[OUTGOING] New player");

                    Send(message[0]+":" + "newplayer");
                    break;
                default:
                    Console.WriteLine("[ERROR] Command unknown");
                    break;
            }
        }

        

        private void checkAliveListener()
        {
            Task.Factory.StartNew(async () =>
            {
                while (connected)
                {
                    if (timeout >= 20)
                    {
                        connected = false;
                        Console.WriteLine("[TIMEOUT] " + address);
                        Program.removeClient(clientId);
                        return;
                    }
                    Send(clientId+":alive");
                    timeout += 1;
                    Thread.Sleep(3000);
                }
            });
        }

        public int getClientId()
        {
            return clientId;
        }

        

        public void Send(string message)
        {
            Program.Reply(message, address);
        }
    }
}
