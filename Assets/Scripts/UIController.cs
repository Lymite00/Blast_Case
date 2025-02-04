using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    [Header("Components")] 
    [SerializeField] private TMP_Text moveCountText;
    [SerializeField] private TMP_Text warningText;
    
    [SerializeField] private Button restartButton;

    [Header("References")] 
    [SerializeField] private GridManager gridManager;
    private void Awake()
    {
        Instance = this;
        GetObjects();
    }

    private void Start()
    {
        UpdateMoveCountText();
        SetMoveWarningText();
        
        restartButton.onClick.AddListener(RestartScene);
    }
    private void RestartScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        GameManager.Instance.currentMoveCount = gridManager.levelManager.totalMoveCount;
        SceneManager.LoadScene(currentSceneIndex);
    }
    private void GetObjects()
    {
        if (restartButton == null)
            restartButton = GameObject.Find("RestartButton").GetComponent<Button>();
        
        if (moveCountText==null)
            moveCountText = GameObject.Find("MoveCountText").GetComponent<TMP_Text>();
        
        if (warningText==null)
            warningText = GameObject.Find("OutOfMovesText").GetComponent<TMP_Text>();

        if (gridManager == null)
            gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
    }

    public void UpdateMoveCountText()
    {
        if (moveCountText!=null && gridManager!=null)
        {
            moveCountText.text = GameManager.Instance.currentMoveCount.ToString();
        }
    }
    public void SetMoveWarningText()
    {
        if (warningText!=null)
        {
            if (GameManager.Instance.currentMoveCount<1)
            {
                StopCoroutine(OpenText(warningText.gameObject));
                StartCoroutine(OpenText(warningText.gameObject));
                warningText.text = "Out of moves!";
            }
            else
            {
                warningText.gameObject.SetActive(false);
                if (restartButton!=null)
                    restartButton.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator OpenText(GameObject textObject)
    {
        if (textObject!=null)
            textObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        textObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        textObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        textObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        textObject.SetActive(true);

        if (restartButton!=null)
            restartButton.gameObject.SetActive(true);
    }
}
