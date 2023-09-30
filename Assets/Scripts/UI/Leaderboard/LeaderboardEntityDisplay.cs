using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text DisplayText;
    [SerializeField] private Color MyColor;

    public ulong ClientId { get; private set; }
    public int Coins { get; private set; }

    private FixedString32Bytes PlayerName;

    public void Initialize(ulong clientId, FixedString32Bytes name, int coins)
    {
        ClientId = clientId;
        PlayerName = name;
        Coins = coins;

        if(clientId == NetworkManager.Singleton.LocalClientId)
        {
            DisplayText.color = MyColor;
        }

        UpdateText();
    }

    public void UpdateCoins(int coins)
    {
        Coins = coins;

        UpdateText();
    }

    public void UpdateText()
    {
        DisplayText.text = $"{transform.GetSiblingIndex() + 1}. {PlayerName} ({Coins})";
    }
}
