using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;

/// <summary>
/// Script to allow for updating the position of game objects in the scene via an external source. The script listens to the port specified for UDP packets that contain 
/// update information. These messages look like:
/// 
/// CLIENT_NAME,OBJECT_NAME,TIMESTAMP,X_POSITION,Y_POSITION,Z_POSITION,X_ROTATION,Y_ROTATION,Z_ROTATION
/// 
/// For example: "client,car,1435,23.23,12.45,42.12,45,0,90"
/// 
/// Items to be updated must have a PositionUpdateBehavior script attached to them. Any updates for items where the object name does not match the name of a PositionUpdateBehavior 
/// will result in a generic cube being instantiated with the PositionUpdateBehavior name attached to it. 
/// </summary>
public class PositionUpdateServer : MonoBehaviour
{
    [Tooltip("Number of port to listen for position updates")]
    public int port;

    private List<PositionUpdateBehavior> updateBehaviors = new List<PositionUpdateBehavior>();
    private Queue<Message> updateMessages = new Queue<Message>();
    private Dictionary<string, Message> updateList = new Dictionary<string, Message>();

    private UdpClient client;

    void Start()
    {
        foreach (PositionUpdateBehavior pub in GameObject.FindObjectsOfType<PositionUpdateBehavior>())
        {
            updateBehaviors.Add(pub);
        }

        client = new UdpClient(port);

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
        while (updateMessages.Count > 0)
        {
            Message m = updateMessages.Dequeue();
            //if we are already tracking an item, check to see if we should upate the position
            if (updateList.ContainsKey(m.objectName))
            {
                Debug.Log("Found " + m.objectName + " in update list");

                //if the current client trying to update it is the same as has been updating previously, and the timestamp is newer, update the position
                if ((updateList[m.objectName].clientName.Equals(m.clientName) && updateList[m.objectName].timestamp < m.timestamp) || updateList[m.objectName] == null)
                {
                    foreach (PositionUpdateBehavior pub in updateBehaviors)
                    {
                        if (pub.name.Equals(m.objectName))
                        {
                            GameObject go = pub.gameObject;
                            go.transform.position = new Vector3(m.xPos, m.yPos, m.zPos);
                            go.transform.rotation = Quaternion.Euler(new Vector3(m.xRot, m.yRot, m.zRot));
                            break;
                        }                        
                    }
                    updateList[m.objectName] = m;
                }
                else
                {
                    if (!updateList[m.objectName].clientName.Equals(m.clientName))
                    {
                        Debug.Log("Cannot update " + m.objectName + "; client " + updateList[m.objectName].clientName + " != " + m.clientName);
                    }
                    else if (updateList[m.objectName].timestamp >= m.timestamp)
                    {
                        Debug.Log("Cannot update " + m.objectName + "; old message");
                    }
                }
            }
            else
            {
                Debug.Log("Didn't find " + m.objectName + " in update list, checking PositionUpdateBehavior list");
                bool foundInList = false;
                foreach (PositionUpdateBehavior behavior in updateBehaviors)
                {
                    if (behavior.name.Equals(m.objectName))
                    {
                        GameObject go = behavior.gameObject;
                        go.transform.position = new Vector3(m.xPos, m.yPos, m.zPos);
                        go.transform.rotation = Quaternion.Euler(new Vector3(m.xRot, m.yRot, m.zRot));
                        updateList.Add(m.objectName, m);

                        foundInList = true;
                        break;
                    }                    
                }

                if (!foundInList)
                {
                    Debug.Log("Creating " + m.objectName);
                    //create object
                    GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    o.AddComponent<PositionUpdateBehavior>();
                    PositionUpdateBehavior pub = o.GetComponent<PositionUpdateBehavior>();
                    pub.name = m.objectName;
                    updateBehaviors.Add(pub);
                    updateList.Add(m.objectName, m);
                }
            }
        }
    }

    private void AcceptCallback(IAsyncResult ar)
    {
        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);
        byte[] recv = client.EndReceive(ar, ref remoteIpEndPoint);
        client.BeginReceive(new AsyncCallback(AcceptCallback), null);

        string message = Encoding.UTF8.GetString(recv);
        Debug.Log("Received message from client: " + message);

        //client_name,object_name,timestamp,x_pos,y_pos,z_pos,x_rot,y_rot,z_rot
        string[] parts = message.Split(new char[] { ',' });

        if (parts.Length == 9)
        {
            Message newMessage = new Message(parts[0], parts[1], int.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]), float.Parse(parts[5]), float.Parse(parts[6]), float.Parse(parts[7]), float.Parse(parts[8]));
            updateMessages.Enqueue(newMessage);
        }
        else
        {
            Debug.Log("Message not correct length, expected 9, received " + parts.Length);
        }
    }

    private class Message
    {
        public string clientName;
        public string objectName;
        public int timestamp;
        public float xPos;
        public float yPos;
        public float zPos;
        public float xRot;
        public float yRot;
        public float zRot;

        public Message(string clientName, string objectName, int timestamp, float xPos, float yPos, float zPos, float xRot, float yRot, float zRot)
        {
            this.clientName = clientName;
            this.objectName = objectName;
            this.timestamp = timestamp;
            this.xPos = xPos;
            this.yPos = yPos;
            this.zPos = zPos;
            this.xRot = xRot;
            this.yRot = yRot;
            this.zRot = zRot;
        }
    }
}
