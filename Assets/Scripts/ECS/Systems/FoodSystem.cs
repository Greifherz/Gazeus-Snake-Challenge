using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class FoodSystem : ComponentSystem, ITickListener
{
    [Inject] private readonly SnakeMovementSystem SnakeMovementSystem;
    [Inject] private readonly TickSystem TickSystem;

    public List<GameObject> SpawnedFoods;

    private GameObject FoodPrefab;

    protected override void OnStartRunning()
    {
        SpawnedFoods = new List<GameObject>();
        TickSystem.AssignTickListener(this,1);
        FoodPrefab = Resources.Load<GameObject>("Prefabs/SnakeFood");
        base.OnStartRunning();
        Enabled = GameStateSystem.Instance != null && GameStateSystem.Instance.IsPlaying;
    }

    protected override void OnUpdate()
    {
        //Do Nothing
    }

    public void Restart()
    {
        for (int i = 0; i < SpawnedFoods.Count; i++)
            GameObject.Destroy(SpawnedFoods[i]);
        SpawnedFoods = new List<GameObject>();
    }

    public void OnTick(int tickCount)
    {
        if (!Enabled) return;

        for(int i = 0; i < SpawnedFoods.Count; i++)
        {
            GameObject CheckFood = SpawnedFoods[i];
            if (SnakeMovementSystem.IsPositionWithinSnake(CheckFood.transform.position ))
            {

                SnakeMovementSystem.FeedSnake(CheckFood.transform.position);
                SpawnedFoods.RemoveAt(i);
                i--;

                GameObject.Destroy(CheckFood);
            }
        }

        if (tickCount % GameSettings.Settings.TicksForSpawningFood != 0) return;
        if (SpawnedFoods.Count >= GameSettings.Settings.MaxFoodCount) return;

        int SpawnRegion = GameSettings.Settings.Bounds -1;

        Vector3 RandomPos = new Vector3(Random.Range(-SpawnRegion, SpawnRegion + 1), 1, Random.Range(-SpawnRegion, SpawnRegion + 1));

        int MaxTries = 60;
        while(MaxTries > 0 && SnakeMovementSystem.IsPositionWithinSnake(RandomPos))
        {
            RandomPos = new Vector3(Random.Range(-SpawnRegion, SpawnRegion + 1), 1, Random.Range(-SpawnRegion, SpawnRegion + 1));
            MaxTries--;
        }

        GameObject SpawnedFood = GameObject.Instantiate(FoodPrefab, RandomPos, Quaternion.identity);
        SpawnedFood.name = SpawnedFood.name.Replace("(Clone)","");
        SpawnedFoods.Add(SpawnedFood);
    }
}
