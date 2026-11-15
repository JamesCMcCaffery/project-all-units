using System.Collections.Generic;
using ProjectAllUnits.Core;
using Unity.Netcode;
using UnityEngine;

namespace ProjectAllUnits.Networking
{
    /// <summary>Spawns the first player to join as the Dispatcher and everyone after as an Officer.</summary>
    public class RoleSpawner : MonoBehaviour
    {
        [SerializeField] NetworkObject dispatcherPrefab;
        [SerializeField] NetworkObject officerPrefab;
        [SerializeField] Transform dispatchDeskSpawn;
        [SerializeField] Transform[] officerSpawns;

        readonly Dictionary<ulong, PlayerRole> roles = new();
        bool dispatcherTaken;
        int nextOfficerSpawn;

        void Start()
        {
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }

        void OnDestroy()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null) return;
            nm.OnServerStarted -= OnServerStarted;
            nm.OnClientConnectedCallback -= OnClientConnected;
            nm.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        void OnServerStarted()
        {
            var nm = NetworkManager.Singleton;
            nm.OnClientConnectedCallback += OnClientConnected;
            nm.OnClientDisconnectCallback += OnClientDisconnected;
            // The host is itself a client; make sure it gets spawned too.
            if (nm.IsHost) OnClientConnected(nm.LocalClientId);
        }

        void OnClientConnected(ulong clientId)
        {
            if (roles.ContainsKey(clientId)) return;

            PlayerRole role = dispatcherTaken ? PlayerRole.Officer : PlayerRole.Dispatcher;
            if (role == PlayerRole.Dispatcher) dispatcherTaken = true;
            roles[clientId] = role;

            NetworkObject prefab;
            Transform spawn;
            if (role == PlayerRole.Dispatcher)
            {
                prefab = dispatcherPrefab;
                spawn = dispatchDeskSpawn;
            }
            else
            {
                prefab = officerPrefab;
                spawn = officerSpawns.Length > 0
                    ? officerSpawns[nextOfficerSpawn++ % officerSpawns.Length]
                    : transform;
            }

            var instance = Instantiate(prefab, spawn.position, spawn.rotation);
            instance.SpawnAsPlayerObject(clientId, destroyWithScene: true);
            Debug.Log($"[RoleSpawner] Client {clientId} joined as {role}");
        }

        void OnClientDisconnected(ulong clientId)
        {
            if (roles.TryGetValue(clientId, out var role) && role == PlayerRole.Dispatcher)
                dispatcherTaken = false;
            roles.Remove(clientId);
        }
    }
}
