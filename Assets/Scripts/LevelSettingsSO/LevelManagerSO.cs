using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Manager", menuName = "Game/Level Manager")]
public class LevelManagerSO : ScriptableObject
{
    [Header("Level Variables")]
    public int rows;
    public int cols;

    public GameObject[] blockPrefabs;

    [Space(10)]
    public int defaultMaxIndex;
    
    [Space(10)]
    public int firstIconMinIndex;
    
    [Space(10)]
    public int secondIconMinIndex;
    
    [Space(10)]
    public int thirdMinIndex;
    
    [Space(10)]
    public int totalMoveCount;
}
