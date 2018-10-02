using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class InputSystem : ComponentSystem
{
    private ComponentGroupArray<SnakeFilter> Snakes;

    private KeyCode ForbiddenKey;

    private struct SnakeFilter
    {
        public Transform Transform;
        public SnakeComponent SnakeComponent;
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        Snakes = GetEntities<SnakeFilter>();
        Enabled = GameStateSystem.Instance != null && GameStateSystem.Instance.IsPlaying;
    }

    public void Restart()
    {
        Snakes = GetEntities<SnakeFilter>();
    }

    protected override void OnUpdate()
    {
        if (Snakes.Length == 0) return;

        ForbiddenKey = GetForbiddenKey();

        if (ForbiddenKey != KeyCode.UpArrow && Input.GetKeyDown(KeyCode.UpArrow))
        {
            for(int i = 0; i < Snakes.Length; i++)
            {
                SnakePartComponent SnakeHead = Snakes[i].SnakeComponent.BodyParts[0];
                SnakeHead.transform.rotation = Quaternion.Euler(0,0,0);
            }
        }
        else if (ForbiddenKey != KeyCode.RightArrow && Input.GetKeyDown(KeyCode.RightArrow))
        {
            for (int i = 0; i < Snakes.Length; i++)
            {
                SnakePartComponent SnakeHead = Snakes[i].SnakeComponent.BodyParts[0];
                SnakeHead.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
        else if (ForbiddenKey != KeyCode.DownArrow && Input.GetKeyDown(KeyCode.DownArrow))
        {
            for (int i = 0; i < Snakes.Length; i++)
            {
                SnakePartComponent SnakeHead = Snakes[i].SnakeComponent.BodyParts[0];
                SnakeHead.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        else if (ForbiddenKey != KeyCode.LeftArrow && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            for (int i = 0; i < Snakes.Length; i++)
            {
                SnakePartComponent SnakeHead = Snakes[i].SnakeComponent.BodyParts[0];
                SnakeHead.transform.rotation = Quaternion.Euler(0, 270, 0);
            }
        }
    }

    private KeyCode GetForbiddenKey()
    {
        SnakeFilter FirstSnake = Snakes[0];

        SnakePartComponent SnakeHead = FirstSnake.SnakeComponent.BodyParts[0];
        SnakePartComponent SnakeFirstBody = FirstSnake.SnakeComponent.BodyParts[1];

        Vector3 FirstBodyForward = SnakeFirstBody.transform.up * -1;

        if(Mathf.RoundToInt(FirstBodyForward.z) == 1)
        {
            return KeyCode.UpArrow;
        }
        else if(Mathf.RoundToInt(FirstBodyForward.z) == -1)
        {
            return KeyCode.DownArrow;
        }
        else if(Mathf.RoundToInt(FirstBodyForward.x) == 1)
        {
            return KeyCode.RightArrow;
        }
        else
        {
            return KeyCode.LeftArrow;
        }
    }
}
