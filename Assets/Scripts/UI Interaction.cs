using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIInteraction : MonoBehaviour
{
    VisualElement root;
    [SerializeField] NetcodeManager netcodeManager;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        //Grab
        Button startHost = root.Q<Button>("StartHost");
        Button startClient = root.Q<Button>("StartClient");

        //Functions
        startHost.clicked += () => StartHost();
        startClient.clicked += () => StartClient();
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        netcodeManager.UpdateTickrates(root.Q<SliderInt>("ServerTickrate").value, root.Q<SliderInt>("ClientTickrate").value);
        root.visible = false;
    }

    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        root.visible = false;
    }
}
