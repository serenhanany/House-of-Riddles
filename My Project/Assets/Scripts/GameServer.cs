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
    // This method is called when the server starts
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server started...");
    }
    // This method is called when the server stops
    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("Server stopped...");
    }
}
