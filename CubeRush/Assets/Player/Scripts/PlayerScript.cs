using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Vector2 fingerDownPos;
    private Vector2 fingerUpPos;

    [Header("Swipe parameters")]
    public bool detectSwipeAfterRelase = false;
    public float SWIPE_DISTANCE = 20f;

    // Control settings
    [Header("Other")]
    public float Duration = 0.1f;
    public float RotateDuration = 1f;
    public float Distance = 2f;
    public GameManager gameManager;
    private bool CanMove = true;
    private bool CanRotate = true;
    public bool CanRayCast = false;
    private Vector3 mustHavePos;
    private float CurrentRotationAngle = 0;

    // Move limits
    [Header("Position limits")]
    [Space(5)]
    public float topLimit = 0.5f;
    public float bottomLimit = -0.5f;
    public float leftLimit = -0.5f;
    public float rightLimit = 0.5f;

    // Other
    public List<GameObject> Shape;

    void Start()
    {
        mustHavePos = new Vector3(0, 0, transform.position.z);
    }

    void Update()
    {
        // Touch control
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == 0)  // Control only with one finger
            {
                if (touch.phase == TouchPhase.Began)
                {
                    fingerUpPos = touch.position;
                    fingerDownPos = touch.position;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    fingerDownPos = touch.position;

                    if (CanMove)
                        DetectSwipe();
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (CanRayCast)
        {
            // Highlight the figure
            HighlightTheFigure();
        }
    }

    // Raycasting
    private void HighlightTheFigure()
    {
        int layerMask = LayerMask.GetMask("Obstacles");

        foreach (GameObject cube in Shape)
        {
            RaycastHit hit;
            
            if (Physics.Raycast(cube.transform.position, transform.forward, out hit, Mathf.Infinity, layerMask))
            {
                hit.collider.GetComponent<CubeScript>().IsChangeColor = true;
            }
        }
    }

    // Check limits methods ---------------------------------
    private bool CheckTopLimit()
    {
        foreach (GameObject cube in Shape)
        {
            if (cube.transform.position.y == topLimit)
                return false;
        }

        return true;
    }

    private bool CheckBottomLimit()
    {
        foreach (GameObject cube in Shape)
        {
            if (cube.transform.position.y == bottomLimit)
                return false;
        }

        return true;
    }

    private bool CheckLeftLimit()
    {
        foreach (GameObject cube in Shape)
        {
            if (cube.transform.position.x == leftLimit)
                return false;
        }

        return true;
    }

    private bool CheckRightLimit()
    {
        foreach (GameObject cube in Shape)
        {
            if (cube.transform.position.x == rightLimit)
                return false;
        }

        return true;
    }
    // ------------------------------------------------------

    private void DetectSwipe()
    {
        if (VerticalMoveValue() > SWIPE_DISTANCE && VerticalMoveValue() > HorizontalMoveValue())
        {
            // Debug.Log("Vertical !!");
            if (fingerDownPos.y - fingerUpPos.y > 0 && CheckTopLimit() && transform.position.y != topLimit)  // Up or Down
            {
                OnSwipeUp();
            }
            else if (fingerDownPos.y - fingerUpPos.y < 0 && CheckBottomLimit() && transform.position.y != bottomLimit)
            {
                OnSwipeDown();
            }

            fingerUpPos = fingerDownPos;
        }
        else if (HorizontalMoveValue() > SWIPE_DISTANCE && HorizontalMoveValue() > VerticalMoveValue())
        {
            // Debug.Log("Horizontal !!");
            if (fingerDownPos.x - fingerUpPos.x > 0 && CheckRightLimit() && transform.position.x != rightLimit)  // Left or Right
            {
                OnSwipeRight();
            }
            else if (fingerDownPos.x - fingerUpPos.x < 0 && CheckLeftLimit() && transform.position.x != leftLimit)
            {
                OnSwipeLeft();
            }

            fingerUpPos = fingerDownPos;
        }
    }

    private float VerticalMoveValue()
    {
        return Mathf.Abs(fingerDownPos.y - fingerUpPos.y);
    }

    private float HorizontalMoveValue()
    {
        return Mathf.Abs(fingerDownPos.x - fingerUpPos.x);
    }

    // ------ Swiping triggers ------

    private void OnSwipeUp()
    {
        // Swipe up
        Vector3 target = CurrentPosY(Distance);
        mustHavePos.y += Distance;
        StartCoroutine(Move(target, Duration));
    }

    private void OnSwipeDown()
    {
        // Swipe down
        Vector3 target = CurrentPosY(-Distance);
        mustHavePos.y -= Distance;
        StartCoroutine(Move(target, Duration));
    }

    private void OnSwipeLeft()
    {
        // Swipe left
        Vector3 target = CurrentPosX(-Distance);
        mustHavePos.x -= Distance;
        StartCoroutine(Move(target, Duration));
    }

    private void OnSwipeRight()
    {
        // Swipe right
        Vector3 target = CurrentPosX(Distance);
        mustHavePos.x += Distance;
        StartCoroutine(Move(target, Duration));
    }

    private Vector3 CurrentPosX(float dist)
    {
        return new Vector3(transform.position.x + dist, transform.position.y, transform.position.z);
    }

    private Vector3 CurrentPosY( float dist)
    {
        return new Vector3(transform.position.x, transform.position.y + dist, transform.position.z);
    }

    // Smooooth !!
    private IEnumerator Move(Vector3 targetPos, float duration)
    {
        CanMove = false;  // Movement lock until the previous movement is complete
        float time = 0;
        float t;
        Vector3 StartPos = transform.position;

        while (time < duration)
        {
            t = time / duration;
            t = 1 - Mathf.Pow(1 - t, 4);
            transform.position = Vector3.Lerp(StartPos, targetPos, t);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        CheckPosition(mustHavePos);
        CanMove = true;
    }

    private IEnumerator Rotate(Quaternion deg, float duration)
    {
        CanRotate = false;
        float time = 0;
        float t;
        Quaternion startValue = transform.rotation;

        while (time < duration)
        {
            t = time / duration;
            t = 1 - Mathf.Pow(1 - t, 4);
            transform.rotation = Quaternion.Lerp(startValue, deg, t);
            time += Time.deltaTime;
            yield return null;
        }

        transform.rotation = deg;
        CanRotate = true;
    }

    private void CheckPosition(Vector3 positionToCheck)
    {
        transform.position = positionToCheck;
    }

    // Rotation methods
    public void RotateLeft()
    {
        if (CanRotate)
        {
            CurrentRotationAngle += 90f;
            StartCoroutine(Rotate(Quaternion.Euler(new Vector3(0, 0, CurrentRotationAngle)), RotateDuration));
        }
    }

    public void RotateRight()
    {
        if (CanRotate)
        {
            CurrentRotationAngle -= 90f;
            StartCoroutine(Rotate(Quaternion.Euler(new Vector3(0, 0, CurrentRotationAngle)), RotateDuration));
        }
    }

    // Method to clear shape
    public void ClearShape()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        transform.position = new Vector3(0, 0, transform.position.z);  // Reset position after complete level
        mustHavePos = transform.position;
    }

    // Collisions
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            gameManager.GameOver();
            Destroy(gameObject);
        }
    }
}
