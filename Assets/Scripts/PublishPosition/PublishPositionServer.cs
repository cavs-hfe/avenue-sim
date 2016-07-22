using UnityEngine;
using System.Collections;
using System.Net;
using System.Text;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

/// <summary>
/// This class is responsible for publishing the position of marked items in the scene. When the script starts it creates a list of items that contain PublishPositionBehavior script. 
/// It also begins listening for a UDP datagram with a "start" message. Once it receives such a message, it adds the IPAddress associated with the message to a list of clients. Every time the update method is called,
/// the program iterates through the clients list, then for each client, it iterates through the list of PublishUpdateBehaviors, sending a message in the format:
/// 
/// CLIENT_NAME,OBJECT_NAME,TIMESTAMP,X_POSITION,Y_POSITION,Z_POSITION,X_ROTATION,Y_ROTATION,Z_ROTATION
/// 
/// For example: "unity,car,1435,23.23,12.45,42.12,45,0,90"
/// 
/// To stop receiving update messages, the client should send a "stop" message to the server. /// 
/// </summary>
public class PublishPositionServer : MonoBehaviour
{

    [Tooltip("Name of client to send with update messages")]
    public string clientName;
    [Tooltip("Number of port to listen for start/stop messages")]
    public int listenPort;
    [Tooltip("Number of port to send update messages")]
    public int sendPort;
    [Tooltip("Number of digits after the decimal for rounding values in update messages")]
    public int decimalPrecision;

    private List<PublishPositionBehavior> updateBehaviors = new List<PublishPositionBehavior>();
    private List<IPAddress> clients = new List<IPAddress>();

    private UdpClient client;
    private Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    void Start()
    {
        foreach (PublishPositionBehavior pub in GameObject.FindObjectsOfType<PublishPositionBehavior>())
        {
            updateBehaviors.Add(pub);
        }

        client = new UdpClient(listenPort);

        try
        {
            client.BeginReceive(new AsyncCallback(AcceptCallback), null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
        }
    }

    void Update()
    {
        foreach (IPAddress address in clients)
        {
            IPEndPoint endpoint = new IPEndPoint(address, sendPort);
            foreach (PublishPositionBehavior pub in updateBehaviors)
            {

                GameObject go = pub.gameObject;
                string message = clientName + "," + pub.name +"," + Math.Round(Time.time, decimalPrecision) + "," + Math.Round(go.transform.position.x, decimalPrecision) + "," + Math.Round(go.transform.position.y, decimalPrecision) + "," + Math.Round(go.transform.position.z, decimalPrecision) + "," + Math.Round(go.transform.rotation.eulerAngles.x, decimalPrecision) + "," + Math.Round(go.transform.rotation.eulerAngles.y, decimalPrecision) + "," + Math.Round(go.transform.rotation.eulerAngles.z, decimalPrecision);
                server.SendTo(Encoding.ASCII.GetBytes(message), endpoint);
            }
        }
    }

    private void AcceptCallback(IAsyncResult ar)
    {
        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
        byte[] recv = client.EndReceive(ar, ref remoteIpEndPoint);
        client.BeginReceive(new AsyncCallback(AcceptCallback), null);

        string message = Encoding.UTF8.GetString(recv);
        Debug.Log("Received message from client: " + message);

        if (message.Equals("start"))
        {
            clients.Add(remoteIpEndPoint.Address);
        }
        else if (message.Equals("stop"))
        {
            clients.Remove(remoteIpEndPoint.Address);
        }
    }
}
