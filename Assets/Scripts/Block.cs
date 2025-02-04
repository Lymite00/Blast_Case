using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 
public class Block : MonoBehaviour
{
    [Header("Actions")] 
    private Action OnBlast;
    private Action OnIdle;
    
    [Header("Variables")]
    public string blockTag;
    public bool isFalling = false;

    [Header("Components")] 
    [SerializeField] private SpriteRenderer currentSprite;
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    [SerializeField] private GameObject blastParticle;
    
    [Header("References")] 
    private GridManager gridManager;
    
    private void Awake()
    {
        currentSprite = GetComponentInChildren<SpriteRenderer>();
        gridManager = FindObjectOfType<GridManager>(); 
        blockTag = gameObject.tag;
    }

    private void OnEnable()
    {
        OnIdle += OnIdleBlock;
        OnBlast += OnBlastBlock;
    }

    private void OnDisable()
    {
        OnIdle -= OnIdleBlock;
        OnBlast -= OnBlastBlock;
    }

    private void Start()
    {
        OnIdle?.Invoke();
        Invoke("FindConnectedBlocks", 0.1f);
    }

    public void FindConnectedBlocks()
    {
        List<Block> connectedBlocks = gridManager.FindConnectedBlocks(this);
        UpdateSprite(connectedBlocks.Count);
    }
    public void FallToPosition(Vector2 targetPos, float duration, Action onComplete = null)
    {
        isFalling = true;
        transform.DOMove(targetPos, duration).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            isFalling = false;
            onComplete?.Invoke();
        });
    }
    private void UpdateSprite(int count)
    {
        // Implemented to check and set new sprites based on number of neighbors
        
        if (gridManager.levelManager!=null)
        {
            if (count < gridManager.levelManager.defaultMaxIndex) 
                currentSprite.sprite = sprites[0];
            else if (count >= gridManager.levelManager.firstIconMinIndex && count < gridManager.levelManager.secondIconMinIndex) 
                currentSprite.sprite = sprites[1];
            else if (count >= gridManager.levelManager.secondIconMinIndex && count < gridManager.levelManager.thirdMinIndex) 
                currentSprite.sprite = sprites[2];
            else if (count>=gridManager.levelManager.thirdMinIndex)
                currentSprite.sprite = sprites[3];
        }
    }

    public void SetSortingOrder(int yPosition)
    {
        //The variable layer system, which changes linearly from top to bottom, was made to prevent objects from overlapping.
        
        if (currentSprite != null && gridManager != null)
        {
            int rows = gridManager.levelManager.rows;
            currentSprite.sortingOrder = (rows - yPosition) * 10; 
        }
    }

    private void TryBlastBlock()
    {
        if (gridManager == null || gridManager.isProcessing) return;

        if (GameManager.Instance.currentMoveCount>0)
        {
            List<Block> connectedBlocks = gridManager.FindConnectedBlocks(this);

            if (connectedBlocks.Count >= 2)
            {
                OnBlast?.Invoke();
                gridManager.RemoveBlocks(connectedBlocks);
            }
        }
    }

    private void OnMouseDown()
    {
        if (isFalling) return;
        TryBlastBlock();
    }

    private void OnBlastBlock()
    {
        // Could be implement animation or visual elements
        Instantiate(blastParticle, transform.position, Quaternion.identity);
        GameManager.Instance.currentMoveCount--;
        CameraController.Instance.ShakeCamera();
        UIController.Instance.UpdateMoveCountText();
        UIController.Instance.SetMoveWarningText();
    }
    private void OnIdleBlock()
    {
        // Could be implement animation or visual elements
    }
}