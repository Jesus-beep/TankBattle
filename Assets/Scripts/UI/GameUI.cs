using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;


    private void Start()
    {
        string clientCode = ClientSingleton.Instance.GameManager.JoinCode;
        string hostCode = HostSingleton.Instance.GameManager.JoinCode;

        if(clientCode.IsNullOrEmpty())
        {
            text.text = hostCode;
        }
        else
        {
            text.text = clientCode;
        }
    }
}
