using HelloWorld;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;


public class HelloWorldEmote : NetworkBehaviour
{
    private enum EmoteType
    {
        Idle,
        Cry,
        Happy,
        Angry
    }

    private EmoteType emoteType;

    public GameObject idleObject;
    public GameObject cryObject;
    public GameObject happyObject;
    public GameObject angryObject;

    private GameObject currentEmoteObject;

    private void Start()
    {
        if (IsOwner)
        {
            HideAllObjectsClientsAndHostRpc();
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                emoteType = EmoteType.Idle;
                SendSwitchEmoteClientAndHostRpc(emoteType);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                emoteType = EmoteType.Cry;
                SendSwitchEmoteClientAndHostRpc(emoteType);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                emoteType = EmoteType.Happy;
                SendSwitchEmoteClientAndHostRpc(emoteType);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                emoteType = EmoteType.Angry;
                SendSwitchEmoteClientAndHostRpc(emoteType);
            }
        }

    }


    [Rpc(SendTo.ClientsAndHost)]
    private void SendSwitchEmoteClientAndHostRpc(EmoteType newEmoteType, RpcParams rpcParams = default)
    {
        emoteType = newEmoteType;
        SwitchEmote(newEmoteType);
    }

    private void SwitchEmote(EmoteType newEmoteType)
    {
        if (currentEmoteObject != null)
        {
            currentEmoteObject.SetActive(false);
        }

        switch (newEmoteType)
        {
            case EmoteType.Idle:
                currentEmoteObject = idleObject;
                break;

            case EmoteType.Cry:
                currentEmoteObject = cryObject;
                break;

            case EmoteType.Happy:
                currentEmoteObject = happyObject;
                break;

            case EmoteType.Angry:
                currentEmoteObject = angryObject;
                break;

            default:
                currentEmoteObject = idleObject;
                break;
        }

        if (currentEmoteObject != null)
        {
            currentEmoteObject.SetActive(true);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void HideAllObjectsClientsAndHostRpc(RpcParams rpcParams = default)
    {
        if (idleObject != null)
        {
            idleObject.SetActive(false);
        }

        if (cryObject != null)
        {
            cryObject.SetActive(false);
        }

        if (happyObject != null)
        {
            happyObject.SetActive(false);
        }

        if (angryObject != null)
        {
            angryObject.SetActive(false);
        }
    }
}
