using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public enum currentState
    {
        GameOver,
        Building,
        GameWon
    }
    public static GameState instance;
    public CanvasGroup gameOverUI;
    public CanvasGroup gameWonUI;

    public currentState curState;
    public Rigidbody2D[] baseNodes;
    private List<SpringJoint2D> baseJoints = new List<SpringJoint2D>();

    private int nodesPlaced;
    private float highestNode;
    public float winPos;

    public GameObject spawner;
    public CameraDrag cameraMove;
    bool hasMovedCam = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null || instance != this)
        {
            instance = this;
        }

        
    }

    // Update is called once per frame
    void Update()
    {

        if (curState == currentState.GameOver)
        {
            gameOverUI.gameObject.SetActive(true);
            
            
            gameWonUI.gameObject.SetActive(false);
        }
        if (curState == currentState.GameWon)
        {
            gameWonUI.gameObject.SetActive(true);
            
            gameOverUI.gameObject.SetActive(false);
        }
        if (curState == currentState.Building)
        {
            gameOverUI.gameObject.SetActive(false);
            gameWonUI.gameObject.SetActive(false);
        }

    }

    public void GameOver()
    {
        if (hasMovedCam) return;
        FadeTo(gameOverUI, 1, 2f);
        cameraMove.MoveWhenLoss();
        spawner.SetActive(false);
        foreach (Rigidbody2D nodes in baseNodes)
        {
            nodes.bodyType = RigidbodyType2D.Dynamic;
            baseJoints.AddRange(nodes.GetComponents<SpringJoint2D>());
        }
        foreach (SpringJoint2D joint in baseJoints)
        {
            if (joint != null)
            {
                joint.breakForce = 0.1f;
            }
        }
        curState = currentState.GameOver;
        hasMovedCam = true;
    }
    public void Won()
    {
        if (hasMovedCam) return;
        FadeTo(gameWonUI, 1, 2f);
        cameraMove.MoveWhenWon();
        spawner.SetActive(false);
        foreach (Rigidbody2D nodes in baseNodes)
        {
            nodes.bodyType = RigidbodyType2D.Dynamic;
            baseJoints.AddRange(nodes.GetComponents<SpringJoint2D>());
        }
        foreach (SpringJoint2D joint in baseJoints)
        {
            joint.breakForce = 0.1f;
        }
        curState = currentState.GameWon;
        hasMovedCam = true;
    }
    public void RegisterNode(Node node)
    {

        nodesPlaced++;
        if (node.transform.position.y > highestNode)
        {
            highestNode = node.transform.position.y;
        }
        if (node.transform.position.y >= winPos)
        {
            Won();
        }
    }


    public void FadeTo(CanvasGroup canvasGroup,float targetAlpha, float duration)
    {
        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, targetAlpha, duration);
    }
}
