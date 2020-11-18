using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public Transform[] movePoints;
    public float moveSpeed;
    public bool isTeleport;
    public float rotationSpeed = 0;
    public float degreeLimit = 0;
    int currPoint = 0;
    float lerpTime = 0;
    float rotateTime = 0;
    float totalDegrees = 0;
    int seasawDirection = 1;
    Quaternion originalRotation;
    public Coroutine move = null;
    Coroutine rotate = null;
    public bool Queued_Movement = false;    // whether the script will automatically move the platform or if they have to be commanded by another script to move.
    
    //[HideInInspector] public bool Moving;
    void OnEnable()
    {
        move = null; rotate = null;
        if (movePoints.Length == 1)
        {
            transform.position = movePoints[0].position;
        }

        if (rotationSpeed != 0)
        {
            originalRotation = transform.localRotation;
        }

        if (degreeLimit != 0)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, degreeLimit);
            seasawDirection = (int) Mathf.Sign(degreeLimit) * -1;
        }

        if (Queued_Movement) { return; }        // player will tell platform when to move, not the system
        toggleMovement();
        toggleRotate();
    }

    IEnumerator lerpPosition()
    {
        float moveDuration = Vector2.Distance(movePoints[currPoint].position, movePoints[(currPoint + 1) % movePoints.Length].position) / moveSpeed;
        while (true)
        {
            if (lerpTime >= moveDuration)
            {
                transform.position = movePoints[currPoint].position;
                currPoint = (currPoint + 1) % movePoints.Length;
                lerpTime = 0;
                if (Queued_Movement && (currPoint == 0 || currPoint == movePoints.Length-1))
                {
                    toggleMovement();
                }
                moveDuration = Vector2.Distance(movePoints[currPoint].position, movePoints[(currPoint + 1) % movePoints.Length].position) / moveSpeed;
            }

            int nextPoint = (currPoint + 1) % movePoints.Length;

            // new Position = current position + delta X
            var DeltaVec = (Vector3)(Vector2.Lerp(movePoints[currPoint].position, movePoints[nextPoint].position, lerpTime / moveDuration) - (Vector2)transform.position); ;
            transform.position += DeltaVec;
            lerpTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator teleportPosition()
    {
        while(true)
        {
            currPoint = (currPoint + 1) % movePoints.Length;
            transform.position = movePoints[currPoint].position;
            yield return new WaitForSeconds(moveSpeed);
        }
    }

    IEnumerator lerpRotation()
    {
        float rotateDuration = 360 / rotationSpeed;
        while (true)
        {
            if (rotateTime >= rotateDuration)
            {
                if (totalDegrees >= 360)
                {
                    transform.localRotation = originalRotation;
                    totalDegrees = 0;
                }
                rotateTime = 0;
            }
            totalDegrees += rotationSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, totalDegrees);
            rotateTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator seasawRotation()
    {
        float abDegreeLimit = Mathf.Abs(degreeLimit);
        float rotateDuration = abDegreeLimit / rotationSpeed;
        while (true)
        {
            if (rotateTime >= rotateDuration)
            {
                if (Mathf.Abs(totalDegrees) >= abDegreeLimit)
                {
                    totalDegrees = abDegreeLimit * seasawDirection;
                    transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, totalDegrees);
                    seasawDirection *= -1;
                }
                rotateTime = 0;
            }
            totalDegrees += rotationSpeed * seasawDirection * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, totalDegrees);
            rotateTime += Time.deltaTime;
            yield return null;
        }
    }

    public void activate()
    {
        if (move == null)
        {
            toggleMovement();
        }
        if (rotate == null)
        {
            toggleRotate();
        }
        gameObject.SetActive(true);
    }

    public void deactivate()
    {
        if (move != null)
        {
            toggleMovement();
        }
        if (rotate != null)
        {
            toggleRotate();
        }
        gameObject.SetActive(false);
    }

    public void toggleMovement()
    {
        if (move != null)
        {
            StopCoroutine(move);
            move = null;
            return;
        }

        if (movePoints.Length > 1 && moveSpeed != 0)
        {
            if (!isTeleport)
            {
                move = StartCoroutine(lerpPosition());
            }
            else
            {
                move = StartCoroutine(teleportPosition());
            }
        }
    }

    public void toggleRotate()
    {
        if (rotate != null)
        {
            StopCoroutine(rotate);
            rotate = null;
            return;
        }

        if (rotationSpeed != 0)
        {
            if (degreeLimit == 0)
            {
                rotate = StartCoroutine(lerpRotation());
            }
            else
            {
                rotate = StartCoroutine(seasawRotation());
            }
        }
    }
}
