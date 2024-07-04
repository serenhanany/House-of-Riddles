using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
public class GameServer : NetworkManager
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server started...");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("Server stopped...");
    }

    /*public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("Client connected: " + conn.address);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("Client disconnected: " + conn.address);
    }*/
}
