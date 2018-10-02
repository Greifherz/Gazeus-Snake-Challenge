using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject
{
    private static GameSettings settings;
    public static GameSettings Settings
    {
        get
        {
            if (settings != null)
                return settings;

            settings = Resources.Load<GameSettings>("Settings/GameSettings");
            return settings;
        }
    }

    public float TickLength = 1;

    public int Bounds;

    public int MaxSnakeSize = 20;

    public int TicksForSpawningFood = 4;

    public int MaxFoodCount = 2;
}
