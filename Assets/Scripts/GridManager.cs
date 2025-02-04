using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("References")]
    private Block[,] grid;
    private List<Block> allBlocks = new List<Block>();
    public LevelManagerSO levelManager;
    
    [Header("Variables")]
    [HideInInspector]
    public bool isProcessing = false;

    void Start()
    {
        GenerateGrid();
        CheckForConnectedBlocks();
        UpdateAllBlockSprites();
    }

    void GenerateGrid()
    {
        grid = new Block[levelManager.rows, levelManager.cols];

        for (int y = 0; y < levelManager.rows; y++)
        {
            for (int x = 0; x < levelManager.cols; x++)
            {
                SpawnBlock(x, y);
            }
        }
    }
    
    private void SpawnBlock(int x, int y)
    {
        Vector2 spawnPos = new Vector2(x, -y + 2);
        int prefabIndex = Random.Range(0, levelManager.blockPrefabs.Length);
        GameObject newBlock = Instantiate(levelManager.blockPrefabs[prefabIndex], spawnPos, Quaternion.identity);
        newBlock.transform.parent = transform;

        Block block = newBlock.GetComponent<Block>();
        block.blockTag = newBlock.tag;
        block.SetSortingOrder(y);
        grid[y, x] = block;
        allBlocks.Add(block);

        block.FallToPosition(new Vector2(x, -y), 0.3f);
    }

    public void RemoveBlocks(List<Block> blocks)
    {
        if (isProcessing) return;
        StartCoroutine(RemoveAndDropBlocks(blocks));
    }

    public IEnumerator RemoveAndDropBlocks(List<Block> blocks)
    {
        isProcessing = true;

        foreach (Block block in blocks)
        {
            if (block == null) continue;
            Vector2 pos = block.transform.position;
            int x = Mathf.RoundToInt(pos.x);
            int y = Mathf.RoundToInt(-pos.y);

            if (grid[y, x] != null)
            {
                Destroy(block.gameObject);
                grid[y, x] = null;
                allBlocks.Remove(block);
            }
        }

        yield return new WaitForSeconds(0.1f);
        DropBlocks();

        yield return new WaitForSeconds(0.1f);
        bool hasConnectedBlocks = CheckForConnectedBlocks();
        if (!hasConnectedBlocks)
        {
            ClearGrid();
            GenerateGrid();
        }

        yield return new WaitForSeconds(0.15f);
        hasConnectedBlocks = CheckForConnectedBlocks();
        if (!hasConnectedBlocks)
        {
            ClearGrid();
            GenerateGrid();
        }
        else
        {
            SpawnMissingBlocks();
            yield return new WaitForSeconds(0.1f);
            UpdateAllBlockSprites();
        }
        yield return new WaitForSeconds(0.15f);

        isProcessing = false;
    }

    private void DropBlocks()
    {
        for (int x = 0; x < levelManager.cols; x++)
        {
            for (int y = levelManager.rows - 1; y >= 0; y--)
            {
                if (grid[y, x] == null)
                {
                    for (int aboveY = y - 1; aboveY >= 0; aboveY--)
                    {
                        if (grid[aboveY, x] != null)
                        {
                            Block fallingBlock = grid[aboveY, x];
                            grid[y, x] = fallingBlock;
                            grid[aboveY, x] = null;
                            fallingBlock.SetSortingOrder(y);

                            fallingBlock.FallToPosition(new Vector2(x, -y), 0.3f);
                            break;
                        }
                    }
                }
            }
        }
    }
    
    private bool CheckForConnectedBlocks()
    {
        foreach (Block block in allBlocks)
        {
            List<Block> connectedBlocks = FindConnectedBlocks(block);
            if (connectedBlocks.Count >= 2)
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnMissingBlocks()
    {
        for (int x = 0; x < levelManager.cols; x++)
        {
            for (int y = 0; y < levelManager.rows; y++)
            {
                if (grid[y, x] == null)
                {
                    SpawnBlock(x, y);
                }
            }
        }
    }

    private void ClearGrid()
    {
        foreach (Block block in allBlocks)
        {
            Destroy(block.gameObject);
        }
        allBlocks.Clear();
        grid = new Block[levelManager.rows, levelManager.cols];
    }

    private void UpdateAllBlockSprites()
    {
        foreach (Block block in allBlocks)
        {
            block.FindConnectedBlocks();
        }
    }

    public List<Block> FindConnectedBlocks(Block startBlock)
    {
        List<Block> connectedBlocks = new List<Block>();
        Stack<Block> stack = new Stack<Block>();
        stack.Push(startBlock);

        while (stack.Count > 0)
        {
            Block current = stack.Pop();
            if (!connectedBlocks.Contains(current))
            {
                connectedBlocks.Add(current);
                foreach (Block neighbor in GetNeighbors(current))
                {
                    if (neighbor != null && neighbor.blockTag == startBlock.blockTag)
                    {
                        stack.Push(neighbor);
                    }
                }
            }
        }
        return connectedBlocks;
    }

    private List<Block> GetNeighbors(Block block)
    {
        List<Block> neighbors = new List<Block>();
        Vector2 pos = block.transform.position;

        AddNeighbor(neighbors, pos + Vector2.up);
        AddNeighbor(neighbors, pos + Vector2.down);
        AddNeighbor(neighbors, pos + Vector2.right);
        AddNeighbor(neighbors, pos + Vector2.left);

        return neighbors;
    }

    private void AddNeighbor(List<Block> neighbors, Vector2 checkPos)
    {
        int x = Mathf.RoundToInt(checkPos.x);
        int y = Mathf.RoundToInt(-checkPos.y);

        if (x >= 0 && x < levelManager.cols && y >= 0 && y < levelManager.rows)
        {
            Block neighbor = grid[y, x];
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }
    }
}
