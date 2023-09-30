using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Image HealPowerBar;

    [Header("Settings")]
    [SerializeField] private int MaxHealPower = 30;
    [SerializeField] private float HealCooldown = 60f;
    [SerializeField] private float HealTickRate = 1f;
    [SerializeField] private int CoinsPerTick = 10;
    [SerializeField] private int HealthperTick = 10;

    private float RemainingCooldown;
    private float TickTimer;
    private List<TankPlayer> TankPlayers = new List<TankPlayer>();

    private NetworkVariable<int> HealPower = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            HealPower.OnValueChanged += HandleHealPowerChange;
            HandleHealPowerChange(0, HealPower.Value);
        }

        if(IsServer)
        {
            HealPower.Value = MaxHealPower;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged -= HandleHealPowerChange;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsServer) { return; }

        if(!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }

        TankPlayers.Add(player);
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(!IsServer) {  return ; }
        if (!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }

        TankPlayers.Remove(player);
    }

    private void Update()
    {
        if(!IsServer) { return; }

        if (RemainingCooldown > 0f)
        {
            RemainingCooldown -= Time.deltaTime;

            if (RemainingCooldown <= 0)
            {
                HealPower.Value = MaxHealPower;
            }
            else
            {
                return;
            }
        }

        TickTimer += Time.deltaTime;
        if(TickTimer >= 1/HealTickRate)
        {
            foreach (TankPlayer player in TankPlayers) 
            {
                if(HealPower.Value == 0) { break; }

                if(player.Health.CurrentHealth.Value == player.Health.maxHealth) { continue; }

                if(player.Wallet.TotalCoins.Value < CoinsPerTick) { continue; }

                player.Wallet.SpendCoins(CoinsPerTick);
                player.Health.RestoreHealth(HealthperTick);

                HealPower.Value -= 1;

                if(HealPower.Value == 0)
                {
                    RemainingCooldown = HealCooldown;
                }
            }

            TickTimer = TickTimer % (1/HealTickRate);
        }
    }

    private void HandleHealPowerChange(int oldHealPower, int newHealPower)
    {
        HealPowerBar.fillAmount = (float)newHealPower / MaxHealPower;
    }
}
