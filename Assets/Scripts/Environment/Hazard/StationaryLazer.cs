using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryLazer : MonoBehaviour
{
    public Animator LazerAnim;
    public float WaitTime;
    public float ShootTime;

    private void Awake()
    {
        
    }

    private IEnumerator LazerCountdown()
    {
        yield return new WaitForSeconds(WaitTime);
        LazerAnim.SetTrigger("Charge");
        LazerAnim.GetComponent<BoxCollider2D>().enabled = true;
        yield return new WaitForSeconds(ShootTime);
        LazerAnim.SetTrigger("StopCharging");
        CallCountdown();
    }

    public void CallCountdown()
    {
        StartCoroutine(LazerCountdown());
    }
    private void OnEnable()
    {
        CallCountdown();
        LazerAnim.GetComponent<BoxCollider2D>().enabled = true;
    }

    private void OnDisable()
    {
        LazerAnim.GetComponent<SpriteRenderer>().sprite = null;
        LazerAnim.GetComponent<BoxCollider2D>().enabled = false;
    }
}
