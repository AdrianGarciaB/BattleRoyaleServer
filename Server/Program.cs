using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace BattleRoyale_Server
{
    class Program
    {
        public static Dictionary<int, ClientThread> clientsList;
        public static UdpListener server;
        public static int idAvailable;
        static void Main(string[] args)
        {
            server = new UdpListener();
            clientsList = new Dictionary<int, ClientThread>();
            idAvailable = 1;
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var received = await server.Receive();
                    Console.WriteLine("[IN] " + "New message");
                    foreach (KeyValuePair<int, ClientThread> entry in clientsList)
                    {
                        if (entry.Value.getSender().Equals(received.Sender)) {
                            Console.WriteLine("[RECV] " + received.Sender);
                            entry.Value.Receive(received.Message);
                            return;
                        }
                    }
                    ClientThread client = new ClientThread(received, idAvailable);
                    clientsList.Add(idAvailable, client);
                    idAvailable++;
                    Console.WriteLine("[NEW] Client: " + received.Sender.ToString());

                }
            }).Wait();

            String consoleInput;

            do
            {
                Console.Write("root@BattleRoyaleServer:~# ");
                consoleInput = Console.ReadLine();

                switch (consoleInput)
                {
                    case "moveuser":
                        Console.Write("ID: ");
                        int id = int.Parse(Console.ReadLine());
                        
                        break;
                    case "online":
                        Console.Write(clientsList.Count);
                        break;
                    case "":
                        break;
                    default:
                        Console.WriteLine("Comando desconocido");
                        break;
                }
            }
            while (consoleInput != "exit");
            
        }

        public static void removeClient(int id)
        {
            clientsList.Remove(id);
        }
    }
}
