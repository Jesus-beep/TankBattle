using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager _networkManager;
    public Action<string> OnClientLeft;
    private Dictionary<ulong, string> ClientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> AuthIdToUserData = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager)
    { 
        this._networkManager = networkManager;

        _networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }

    private void OnNetworkReady()
    {
        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if(ClientIdToAuth.TryGetValue(clientId, out string authId))
        {
            ClientIdToAuth.Remove(clientId);
            AuthIdToUserData.Remove(authId);

            OnClientLeft?.Invoke(authId);
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        ClientIdToAuth[request.ClientNetworkId] = userData.UserAuthID;
        AuthIdToUserData[userData.UserAuthID] = userData;

        response.Approved = true;
        response.Position = SpawnPoint.GetRandomSpawnPos();
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true;
    }

    public UserData GetUserDataByClientId(ulong clientId)
    {
        if(ClientIdToAuth.TryGetValue(clientId,out string authId)) 
        {
            if(AuthIdToUserData.TryGetValue(authId, out UserData userData))
            {
                return userData;
            }
        }

        return null;
    }

    public void Dispose()
    {
        if(_networkManager != null)
        {
            _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            _networkManager.OnServerStarted -= OnNetworkReady;
        }

        if(_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }
}
