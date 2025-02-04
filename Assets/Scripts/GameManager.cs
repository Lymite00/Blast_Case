using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{ 
    public static GameManager Instance { get; private set; }
    
    
    [Header("References")] 
    [SerializeField] private GridManager gridManager;
    
    [Header("States")] 
    public GameStates currentState;
    
    [Header("Variables")] 
    public int currentMoveCount;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
        
        GetObjects();
       
    }
    
    private void GetObjects()
    {
        if (gridManager == null)
            gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        
        if(gridManager!=null)
            currentMoveCount = gridManager.levelManager.totalMoveCount;
    }
    public enum GameStates
    {
        Win,
        Lose,
        Pause
    }

    public void StateController(GameStates state)
    {
        currentState = state;
        switch (currentState)
        {
            case GameStates.Win:
                break;
            case GameStates.Lose:
                break;
            case GameStates.Pause:
                break;
        }
    }

    
}
