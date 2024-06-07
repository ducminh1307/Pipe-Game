using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class LevelData : ScriptableObject
{

    public int row;
    public int column;
    public List<int> data;

}
