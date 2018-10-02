using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeComponent : MonoBehaviour
{
    public int Size = 3;
    public GameObject BodyPrefab;
    public List<SnakePartComponent> BodyParts;
    public bool Increase = false;
}
