using System.Collections.Generic;
using ProjectAllUnits.Core;
using ProjectAllUnits.Player;
using Unity.Netcode;
using UnityEngine;

namespace ProjectAllUnits.Networking
{
    /// <summary>Spawns each joining player and assigns the first as Dispatcher and the rest as Officers.</summary>
    public class RoleSpawner : MonoBehaviour
    {
        [SerializeField] NetworkObject playerPrefab;
        [SerializeField] Transform dispatchRoomSpawn;
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

            Transform spawn = SpawnPointFor(role);
            var player = Instantiate(playerPrefab, spawn.position, spawn.rotation);
            player.SpawnAsPlayerObject(clientId, destroyWithScene: true);
            player.GetComponent<PlayerRoleState>().AssignRole(role);
            Debug.Log($"[RoleSpawner] Client {clientId} joined as {role}");
        }

        void OnClientDisconnected(ulong clientId)
        {
            if (roles.TryGetValue(clientId, out var role) && role == PlayerRole.Dispatcher)
                dispatcherTaken = false;
            roles.Remove(clientId);
        }

        Transform SpawnPointFor(PlayerRole role)
        {
            if (role == PlayerRole.Dispatcher) return dispatchRoomSpawn;
            if (officerSpawns.Length == 0) return transform;
            return officerSpawns[nextOfficerSpawn++ % officerSpawns.Length];
        }
    }
}
