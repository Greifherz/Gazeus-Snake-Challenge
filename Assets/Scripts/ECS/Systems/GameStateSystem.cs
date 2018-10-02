using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class GameStateSystem : ComponentSystem
{
    public static GameStateSystem Instance;
    
    [Inject] private readonly InputSystem InputSystem;
    [Inject] private readonly SnakeMovementSystem SnakeMovementSystem;
    [Inject] private readonly FoodSystem FoodSystem;
    [Inject] private readonly TickSystem TickSystem;

    public bool IsPlaying = false;

    public static void Start()
    {
        RestartGame();
    }

    public static void RestartGame()
    {
        Instance.IsPlaying = true;
        Instance.Restart();
    }

    public void Restart()
    {
        SnakeMovementSystem.Restart();
        InputSystem.Restart();
        FoodSystem.Restart();
        TickSystem.Restart();
        EnableSystems();
    }

    public static void EndGame(bool win = false)
    {
        Instance.DisableSystems();
        Instance.IsPlaying = false;

        UIBootstrapComponent.Instance.GameStatePanel.SetActive(true);
        if (win)
        {
            Debug.Log("Game Over - You win.");
            UIBootstrapComponent.Instance.GameStateText.text = "Game Over " + System.Environment.NewLine + " You Win";
        }
        else
        {
            UIBootstrapComponent.Instance.GameStateText.text = "Game Over " + System.Environment.NewLine + " You Lose";
            Debug.Log("Game Over - You lose.");
        }
    }

    private void EnableSystems()
    {
        InputSystem.Enabled = true;
        FoodSystem.Enabled = true;
        SnakeMovementSystem.Enabled = true;
        TickSystem.Enabled = true;
        Enabled = true;
    }

    private void DisableSystems()
    {
        InputSystem.Enabled = false;
        FoodSystem.Enabled = false;
        SnakeMovementSystem.Enabled = false;
        TickSystem.Enabled = false;
        Enabled = false;
    }

    protected override void OnStartRunning()
    {
        Instance = this;
        base.OnStartRunning();
    }

    protected override void OnUpdate()
    {
        //Do Nothing
    }
}
