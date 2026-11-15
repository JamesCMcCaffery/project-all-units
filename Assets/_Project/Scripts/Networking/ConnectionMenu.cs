using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace ProjectAllUnits.Networking
{
    /// <summary>
    /// Throwaway debug menu for step 1: Host / Join by IP using Unity Transport.
    /// Replaced by Steam lobbies (Facepunch transport) later.
    /// </summary>
    public class ConnectionMenu : MonoBehaviour
    {
        string address = "127.0.0.1";
        ushort port = 7777;

        void OnGUI()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null) return;

            GUILayout.BeginArea(new Rect(10, 10, 260, 200), GUI.skin.box);
            if (!nm.IsClient && !nm.IsServer)
            {
                GUILayout.Label("PROJECT ALL UNITS - dev menu");
                GUILayout.Label("Address");
                address = GUILayout.TextField(address);
                if (GUILayout.Button("Host (you are Dispatch)"))
                {
                    SetConnection("0.0.0.0");
                    nm.StartHost();
                }
                if (GUILayout.Button("Join (you are an Officer)"))
                {
                    SetConnection(address);
                    nm.StartClient();
                }
            }
            else
            {
                GUILayout.Label(nm.IsHost ? "Hosting" : "Connected");
                GUILayout.Label($"Players: {(nm.IsServer ? nm.ConnectedClientsIds.Count.ToString() : "?")}");
                if (GUILayout.Button("Disconnect")) nm.Shutdown();
            }
            GUILayout.EndArea();
        }

        void SetConnection(string ip)
        {
            var transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            transport.SetConnectionData(ip == "0.0.0.0" ? "127.0.0.1" : ip, port, ip == "0.0.0.0" ? "0.0.0.0" : null);
        }
    }
}
