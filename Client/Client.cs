using Assets.Scripts.Multiplayer;
using System.Threading.Tasks;
using UnityEngine;

public class Client : MonoBehaviour
{
   

    public GameObject player;
    private UdpUser udpUser;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting...");
        udpUser = UdpUser.ConnectTo("127.0.0.1", 32123);
        udpUser.Send("Conected?");
        Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                var received = await udpUser.Receive();
                switch (received.Message)
                {
                    case "alive":
                        udpUser.Send("alive");
                        break;
                }
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
       //Send(player.transform.position.ToString());
    }

   
}
