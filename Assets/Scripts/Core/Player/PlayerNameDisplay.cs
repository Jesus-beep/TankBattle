using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TankPlayer Player;
    [SerializeField] private TMP_Text DisplayNameText;
    private void Start()
    {
        HandlePlayerNameChanged(string.Empty, Player.PlayerName.Value);

        Player.PlayerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        DisplayNameText.text = newName.ToString();
    }

    private void OnDestroy()
    {
        Player.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
    }
}
