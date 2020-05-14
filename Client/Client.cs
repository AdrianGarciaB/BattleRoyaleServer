using Assets.Scripts.Multiplayer;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Client : MonoBehaviour
{
   

    public GameObject playerToInstanciate;
    public Dictionary<int, GameObject> playerEnemies;
    private UdpUser input;
    private UdpUser output;
    private int userId;
    private bool connected;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting...");
        input = UdpUser.ConnectTo("127.0.0.1", 32123);


        input.Send("0:init");

        playerEnemies = new Dictionary<int, GameObject>();

        connected = true;

        Task.Factory.StartNew(async () =>
        {
            Debug.Log("Waiting for userId...");

            var received = await input.Receive();
            userId = int.Parse(received.Message);

            Debug.Log("UserId is: " + userId);

            while (connected)
            {
                Debug.Log("Waiting...");
                received = await input.Receive();
                string[] response = received.Message.Split(':');

                switch (response[1])
                {
                    case "alive":
                        input.Send(userId + ":alive");
                        break;
                    case "newplayer":
                        Debug.Log("New player");
                        GameObject player = Instantiate(playerToInstanciate);
                        playerEnemies.Add(int.Parse(response[1]), player);
                        break;
                    default:
                        Debug.Log("Unknown packet: " );
                        break;
                }
            }
        }).Wait();
    }

    // Update is called once per frame
    void Update()
    {
       //Send(player.transform.position.ToString());
    }

    void OnDestroy()
    {
        connected = false;
    }


}
