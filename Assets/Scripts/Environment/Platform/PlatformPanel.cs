using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPanel : MonoBehaviour
{
    private Animator PlatAnim;
    private EdgeCollider2D edge;

    public bool Automatic;
    [Tooltip("Use This to create wave effects")] public float InitialWaitTime = 1f;
    public float WaitTime;

    private void Awake()
    {
        PlatAnim = GetComponent<Animator>();
        edge = GetComponent<EdgeCollider2D>();
    }

    private IEnumerator Loop()
    {
        yield return new WaitForSeconds(InitialWaitTime);
        while (true)
        {
            PlatAnim.SetTrigger("Open");
            yield return new WaitForSeconds(WaitTime);
        }
    }

    public void ResetTrigger()
    {
        PlatAnim.ResetTrigger("Open");
    }

    private void OnEnable()
    {
        edge.enabled = true;
        if (Automatic)
            StartCoroutine(Loop());
    }
    private void OnDisable()
    {
        edge.enabled = true;
        ResetTrigger();
    }
}
