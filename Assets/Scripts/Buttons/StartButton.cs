using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{

    public void OnClick()
    {
        //Hide Lobby
        UIBootstrapComponent.Instance.LobbyPanel.SetActive(false);

        //Hide GameState
        UIBootstrapComponent.Instance.GameStatePanel.SetActive(false);

        //TriggerStart
        GameStateSystem.Start();
    }
}
