using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLasso : MonoBehaviour
{
    public ThrowLasso grapplingGun;

    public Vector2 Force;
    public float MaximumDistance;
    [HideInInspector] public Transform Parent;
    [HideInInspector] public Transform Latch;
    private LineRenderer Lasso_LineRenderer;
    private bool isGrappling = false;
    [Header("Draw Settings")]
    public bool DrawRopeStraight = true;
    [SerializeField] int precision = 30;
    [Range(0, 100)] [SerializeField] private float straightenLineSpeed = 3.1f;     // speed that it takes to straighten out the rope.

    [Header("Animation:")]
    public AnimationCurve ropeAnimationCurve;
    [SerializeField] [Range(0.01f, 4)] private float WaveSize = 1.1f;
    float _waveSize;
    float moveTime = 0;

    private void Awake()
    {
        Lasso_LineRenderer = GetComponent<LineRenderer>();
        Lasso_LineRenderer.enabled = false;
        Lasso_LineRenderer.positionCount = precision;
    }

    private void OnEnable()
    {
        moveTime = 0;
        Lasso_LineRenderer.enabled = true;
        Lasso_LineRenderer.positionCount = precision;
        _waveSize = WaveSize;
        DrawRopeStraight = true;
        LinePointToFirePoint();
    }

    private void OnDisable()
    {
        Lasso_LineRenderer.enabled = false;
        isGrappling = false;
        grapplingGun.ResetPriorityTarget();
    }

    void LinePointToFirePoint()
    {
        for (int i = 0; i < precision; i++)
        {
            Lasso_LineRenderer.SetPosition(i, grapplingGun.transform.position);
        }
    }

    void Update()
    {
        if (grapplingGun.GetPrioritizedTarget() != null)
        {
            moveTime += Time.deltaTime;
            DrawRope();
        }
    }

    void DrawRope()
    {
        if (!isGrappling)
        {
            isGrappling = true;
            // modify waveSize here.
            float _distance = Vector2.Distance(grapplingGun.GetPrioritizedTarget().position, (Vector2)transform.position);
            _waveSize = Mathf.Clamp(WaveSize * _distance / 5,0,WaveSize);
            //print("new Wavesize" + _waveSize);
            // don't bother drawing curves if the rope isn't long enough.
        }
        if (_waveSize > 0)
        {
            _waveSize -= Time.deltaTime * straightenLineSpeed;
            DrawRopeWaves();
        }
        else
        {
            _waveSize = 0;
            DrawRopeNoWaves();
        }
        
    }

    void DrawRopeWaves()
    {
        Vector2 grapplePoint = grapplingGun.GetPrioritizedTarget().position;
        for (int i = 0; i < precision; i++)
        {
            Vector2 DistanceVector = (Vector2)(grapplePoint - (Vector2)grapplingGun.transform.position);
            float delta = (float)i / ((float)precision - 1f);
            Vector2 offset = Vector2.Perpendicular(DistanceVector).normalized * ropeAnimationCurve.Evaluate(delta) * _waveSize;
            Vector2 targetPosition = Vector2.Lerp(grapplingGun.transform.position, grapplePoint, delta) + offset;
            Lasso_LineRenderer.SetPosition(i, targetPosition);
        }
    }

    void DrawRopeNoWaves()
    {
        Lasso_LineRenderer.positionCount = 2;
        Lasso_LineRenderer.SetPosition(0, grapplingGun.transform.position);
        Lasso_LineRenderer.SetPosition(1, grapplingGun.GetPrioritizedTarget().position);
    }

    public bool IsGrappling()
    {
        return isGrappling;
    }
}
