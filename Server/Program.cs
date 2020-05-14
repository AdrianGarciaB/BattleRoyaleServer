using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
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
                    try { 
                    var received = await server.Receive();
                    string[] response = received.Message.Split(":");
                    Console.WriteLine("[IN] " + "New message");
                    if (response[0] != "0")
                    {
                        
                        foreach (KeyValuePair<int, ClientThread> entry in clientsList)
                        {
                            if (entry.Value.getClientId() == int.Parse(response[0]))
                            {
                                Console.WriteLine("[RECV] " + received.Sender);
                                entry.Value.Receive(response);
                            }
                        }
                        
                    }
                    else {
                        ClientThread client = new ClientThread(received.Sender, idAvailable);
                            /*
                        foreach (KeyValuePair<int, ClientThread> entry in clientsList)
                        {
                            entry.Value.Receive(new string[]{idAvailable.ToString(), "newplayer"});
                        }*/
                        clientsList.Add(idAvailable, client);
                        idAvailable++;
                        Console.WriteLine("[NEW] Client: " + received.Sender.ToString());
                    }
                        
                    }
                    catch (TimeoutException te)
                    {
                        Console.WriteLine("Timeout!");
                    }
                    catch (AggregateException ae)
                    {
                        Console.WriteLine("Disconected!");
                    }

                }

            }).Wait();

            Console.ReadKey();
            
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
                        Console.WriteLine(clientsList.Count);
                        break;
                    case "":
                    case "exit":
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

        public static void Reply(string message, IPEndPoint address)
        {
              server.Reply(message, address);
            
        }
    }
}
