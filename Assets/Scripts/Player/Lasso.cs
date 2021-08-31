using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasso : MonoBehaviour
{
    public ThrowLasso grapplingGun;

    public Vector2 Force;
    public float MaximumDistance;
    [HideInInspector] public Transform Parent;
    [HideInInspector] public Transform Latch;
    private LineRenderer Lasso_Segment;
    private int percision = 120;
    private bool isGrappling = false;
    /*
     *     [Header("General refrences:")]
    public GrapplingGun grapplingGun;
    [SerializeField] LineRenderer m_lineRenderer;

    [Header("General Settings:")]
    [SerializeField] private int percision = 20;
    [Range(0, 100)][SerializeField] private float straightenLineSpeed = 4;

    [Header("Animation:")]
    public AnimationCurve ropeAnimationCurve;
    [SerializeField] [Range(0.01f, 4)] private float WaveSize = 20;
    float waveSize;

    [Header("Rope Speed:")]
    public AnimationCurve ropeLaunchSpeedCurve;
    [SerializeField] [Range(1, 50)] private float ropeLaunchSpeedMultiplayer = 4;

    float moveTime = 0;

    [SerializeField]public bool isGrappling = false;
    
    bool drawLine = true;
    bool straightLine = true;
     * */
    float moveTime = 0;

    private void Awake()
    {
        Lasso_Segment = GetComponent<LineRenderer>();
        Lasso_Segment.enabled = false;
        Lasso_Segment.positionCount = percision;
    }

    private void OnEnable()
    {
        moveTime = 0;
        Lasso_Segment.enabled = true;
        Lasso_Segment.positionCount = percision;
        //waveSize = WaveSize;
        //straightLine = false;
        LinePointToFirePoint();
    }

    private void OnDisable()
    {
        Lasso_Segment.enabled = false;
        isGrappling = false;
    }

    void LinePointToFirePoint()
    {
        for (int i = 0; i < percision; i++)
        {
            Lasso_Segment.SetPosition(i, grapplingGun.transform.position);
        }
    }

    void Update()
    {
        moveTime += Time.deltaTime;

        DrawRope();
        
    }

    void DrawRope()
    {
        /*
        if (!straightLine)
        {
            if (m_lineRenderer.GetPosition(percision - 1).x != grapplingGun.grapplePoint.x)
            {
                DrawRopeWaves();
            }
            else
            {
                straightLine = true;
            }
        }
        else
        {
        */
        if (!isGrappling)
        {
            grapplingGun.Grapple(); // we dont have this yet.
            isGrappling = true;
        }
        /*
        if (waveSize > 0)
        {
            waveSize -= Time.deltaTime * straightenLineSpeed;
            DrawRopeWaves();
        }
        else
        {*/
            //waveSize = 0;
        DrawRopeNoWaves();
        //}
        //}
    }
    /*
    void DrawRopeWaves()
    {
        for (int i = 0; i < percision; i++)
        {
            float delta = (float)i / ((float)percision - 1f);
            Vector2 offset = Vector2.Perpendicular(grapplingGun.DistanceVector).normalized * ropeAnimationCurve.Evaluate(delta) * waveSize;
            Vector2 targetPosition = Vector2.Lerp(grapplingGun.firePoint.position, grapplingGun.grapplePoint, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(grapplingGun.firePoint.position, targetPosition, ropeLaunchSpeedCurve.Evaluate(moveTime) * ropeLaunchSpeedMultiplayer);

            m_lineRenderer.SetPosition(i, currentPosition);
        }
    }*/

    void DrawRopeNoWaves()
    {
        Lasso_Segment.positionCount = 2;
        Lasso_Segment.SetPosition(0, grapplingGun.transform.position);
        Lasso_Segment.SetPosition(1, grapplingGun.GetPrioritizedTarget().position);
    }
}
