using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private BountyCoin CoinPrefab;
    [SerializeField] private Health Health;

    [Header("Settings")]
    [SerializeField] private float CoinSpread = 3f;
    [SerializeField] private float BountyPercentage = 0.5f;
    [SerializeField] private int BountyCoinCount = 10;
    [SerializeField] private int MinBountyCoinValue = 5;

    [SerializeField] private LayerMask LayerMask;
    private Collider2D[] CoinBuffer = new Collider2D[1];
    private float CoinRadius;

    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        CoinRadius = CoinPrefab.GetComponent<CircleCollider2D>().radius;
        Health.OnDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        Health.OnDie -= HandleDie;
    }

    private void HandleDie(Health health)
    {
        int bountyValue = (int)(TotalCoins.Value * BountyPercentage);

        int bountyCoinValue = bountyValue / BountyCoinCount;

        if (bountyCoinValue < MinBountyCoinValue) { return; }

        for (int i = 0; i< BountyCoinCount; i++)
        {
            BountyCoin coinInstance = Instantiate(CoinPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.SetValue(bountyCoinValue);
            coinInstance.NetworkObject.Spawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Coin>(out Coin coinComp))
        {
            int coinValue = coinComp.Collect();

            if(!IsServer) { return; }

            TotalCoins.Value += coinValue;
        }
    }

    public void SpendCoins(int costToFire)
    {
        TotalCoins.Value -= costToFire; 
    }

    private Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * CoinSpread;
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, CoinRadius, CoinBuffer, LayerMask);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
