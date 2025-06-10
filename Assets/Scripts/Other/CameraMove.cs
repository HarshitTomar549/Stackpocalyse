using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public float dragSpeed = 0.01f;
    private Vector3 lastMousePosition;
    private bool isDragging;
    public Vector2 bounds;

    public Transform offset;
    private bool inBoundsUP;
    private bool inBoundsDown;
    private bool inBoundsLeft;
    private bool inBoundsRight;

    bool enableInput = true;

    public float zoomedOut;

    public Vector3 losePos;
    public Vector3 wonPos;

    public float animationTime = 1;
    public Animator animator;
    
    void Update()
    {
        if(!enableInput) return;

        // Begin dragging with middle mouse
        if (Input.GetMouseButtonDown(2))
        {
            lastMousePosition = Input.mousePosition;
            isDragging = true;
        }
        // Stop dragging
        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 worldDelta = Camera.main.ScreenToWorldPoint(lastMousePosition) - Camera.main.ScreenToWorldPoint(lastMousePosition + delta);

            Vector3 targetPos = transform.position + new Vector3(worldDelta.x, worldDelta.y, 0f);
            inBoundsLeft = targetPos.x > -bounds.x + offset.position.x;
            inBoundsRight = targetPos.x < bounds.x + offset.position.x;
            inBoundsDown = targetPos.y > -bounds.y + offset.position.y;
            inBoundsUP = targetPos.y < bounds.y + offset.position.y;

            if (inBoundsLeft &&
            inBoundsRight )
            {
                transform.position += new Vector3(worldDelta.x, 0f, 0f);
            }
            if (inBoundsDown &&
            inBoundsUP)
            {
                transform.position += new Vector3(0f, worldDelta.y, 0f);
            }

            lastMousePosition = Input.mousePosition;
        }
    }
    public void MoveWhenWon()
    {
        enableInput = false;
        animator.SetTrigger("Zoom");
        LeanTween.move(gameObject, wonPos, animationTime).setEaseOutQuad();
    }
    public void MoveWhenLoss()
    {
        enableInput = false;
        animator.SetTrigger("Zoom");
        LeanTween.move(gameObject, losePos, animationTime).setEaseOutQuad();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(offset.position, bounds);
    }
}
