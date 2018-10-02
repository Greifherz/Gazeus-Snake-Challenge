using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBootstrapComponent : MonoBehaviour
{
    private static UIBootstrapComponent instance;
    public static UIBootstrapComponent Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public Text SnakeSizeText;

    public GameObject GameStatePanel;
    public Text GameStateText;

    public GameObject LobbyPanel;

}
