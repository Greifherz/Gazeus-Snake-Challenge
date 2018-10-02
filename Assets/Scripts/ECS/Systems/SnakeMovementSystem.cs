using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Linq;
using System;

public class SnakeMovementSystem : ComponentSystem, ITickListener
{    
    [Inject] private readonly InputSystem InputSystem;
    [Inject] private readonly TickSystem TickSystem;

    private ComponentGroupArray<SnakeFilter> Snakes;
    private GameObject StartingSnakePrefab;

    private struct SnakeFilter
    {
        public Transform Transform;
        public SnakeComponent SnakeComponent;
    }

    protected override void OnStartRunning()
    {
        TickSystem.AssignTickListener(this);
        base.OnStartRunning();
        Enabled = GameStateSystem.Instance != null && GameStateSystem.Instance.IsPlaying;
    }

    protected override void OnUpdate()
    {
        //Do Nothing
    }

    public void Restart()
    {
        for(int i = 0; i < Snakes.Length; i++)
        {
            GameObject.Destroy(Snakes[i].SnakeComponent.gameObject);
        }

        if (StartingSnakePrefab == null)
            StartingSnakePrefab = Resources.Load<GameObject>("Prefabs/Snake");

        GameObject Snake = GameObject.Instantiate(StartingSnakePrefab, Vector3.zero, Quaternion.identity);
        Snake.name = Snake.name.Replace("(Clone)","");

        Snakes = GetEntities<SnakeFilter>();
    }

    public void OnTick(int tickCount)
    {
        if (!Enabled) return;
        //MoveSnake
        for (int i = 0; i < Snakes.Length; i++)
        {
            MoveSnake(Snakes[i].SnakeComponent);
        }

        UIBootstrapComponent.Instance.SnakeSizeText.text = "SnakeSize - " + Snakes[0].SnakeComponent.Size.ToString() ;
    }

    private void MoveSnake(SnakeComponent snake)
    {
        bool TooBig = false;
        for(int i = snake.BodyParts.Count - 1; i >= 0; i--)
        {
            SnakePartComponent IteratingPart = snake.BodyParts[i];
            if(snake.Increase && i > 1)
            {
                //Do Nothing
            }
            else if(snake.Increase && i == 1)
            {
                GameObject AddedPart = GameObject.Instantiate<GameObject>(snake.BodyPrefab);
                AddedPart.name = AddedPart.name.Replace("(Clone)", "");
                AddedPart.transform.SetParent(snake.transform);
                AddedPart.transform.position = IteratingPart.transform.position;
                AddedPart.transform.up = IteratingPart.transform.up;

                snake.BodyParts.Insert(1, AddedPart.GetComponent<SnakePartComponent>());
                snake.Increase = false;
                snake.Size++;
                i++;

                if (snake.Size >= GameSettings.Settings.MaxSnakeSize)
                    TooBig = true;
            }
            else
            {
                if(i > 0)
                {
                    int NextPartIndex = i - 1;
                    SnakePartComponent NextPart = snake.BodyParts[NextPartIndex];
                    Vector3 Forward = NextPart.transform.position - IteratingPart.transform.position;
                    IteratingPart.transform.position += Forward;
                }
                else
                {
                    IteratingPart.transform.position += IteratingPart.transform.forward;
                }
            }
        }

        NormalizePartsRotation(snake);

        if(CheckForCollisionWithSelf(snake) || CheckForBoundsCollision(snake))
        {//EndGame
            GameStateSystem.EndGame();
        }
        else if (TooBig)
            GameStateSystem.EndGame(true);

    }

    public void FeedSnake(Vector3 eatPosition)
    {
        SnakeComponent EatingSnake = GetSnakeAt(eatPosition);
        EatingSnake.Increase = true;
    }

    private void NormalizePartsRotation(SnakeComponent snake)
    {
        for (int i = 1 ; i < snake.BodyParts.Count; i++)
        {
            snake.BodyParts[i].transform.up = snake.BodyParts[i - 1].transform.position - snake.BodyParts[i].transform.position;
        }
    }

    private bool CheckForCollisionWithSelf(SnakeComponent snake)
    {
        List<Vector3> PartPositions = new List<Vector3>();
        for (int i = 0; i < snake.BodyParts.Count; i++)
        {
            SnakePartComponent CheckPart = snake.BodyParts[i];
            Vector3 PartPosition = new Vector3(Mathf.RoundToInt(CheckPart.transform.position.x), Mathf.RoundToInt(CheckPart.transform.position.y), Mathf.RoundToInt(CheckPart.transform.position.z));
            if (!PartPositions.Contains(PartPosition))
                PartPositions.Add(PartPosition);
            else
                return true;
        }
        return false;
    }

    private bool CheckForBoundsCollision(SnakeComponent snake)
    {
        try
        {            
            return snake.BodyParts.FirstOrDefault(x => Mathf.Abs(x.transform.position.x) >= GameSettings.Settings.Bounds || Mathf.Abs(x.transform.position.z) >= GameSettings.Settings.Bounds) != null;
        }
        catch(System.Exception ex)
        {
            Debug.LogError("Could not get GameSettings from project. Error : " + ex.Message + System.Environment.NewLine + ex.StackTrace);
        }
        return false;
    }

    private SnakeComponent GetSnakeAt(Vector3 position)
    {
        Vector3 CheckPosition = new Vector3(Mathf.RoundToInt(position.x), 1, Mathf.RoundToInt(position.z));

        for (int SnakeIndex = 0; SnakeIndex < Snakes.Length; SnakeIndex++)
        {
            for (int PartIndex = 0; PartIndex < Snakes[SnakeIndex].SnakeComponent.BodyParts.Count; PartIndex++)
            {
                SnakePartComponent CheckPart = Snakes[SnakeIndex].SnakeComponent.BodyParts[PartIndex];
                Vector3 PartPosition = new Vector3(Mathf.RoundToInt(CheckPart.transform.position.x), Mathf.RoundToInt(CheckPart.transform.position.y), Mathf.RoundToInt(CheckPart.transform.position.z));
                if (PartPosition.Equals(CheckPosition))
                    return Snakes[SnakeIndex].SnakeComponent;
            }
        }

        return null;
    }

    public bool IsPositionWithinSnake(Vector3 position)
    {
        List<Vector3> PartPositions = new List<Vector3>();

        for (int SnakeIndex =  0; SnakeIndex < Snakes.Length; SnakeIndex++)
        {
            for(int PartIndex = 0; PartIndex < Snakes[SnakeIndex].SnakeComponent.BodyParts.Count; PartIndex++)
            {
                SnakePartComponent CheckPart = Snakes[SnakeIndex].SnakeComponent.BodyParts[PartIndex];
                Vector3 PartPosition = new Vector3(Mathf.RoundToInt(CheckPart.transform.position.x), Mathf.RoundToInt(CheckPart.transform.position.y), Mathf.RoundToInt(CheckPart.transform.position.z));
                if (!PartPositions.Contains(PartPosition))
                    PartPositions.Add(PartPosition);
            }
        }

        Vector3 CheckPosition = new Vector3(Mathf.RoundToInt(position.x), 1, Mathf.RoundToInt(position.z));

        return PartPositions.Contains(CheckPosition);
    }
}
