using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public enum e_ProjectileType { Rocket = 0, Mine = 1};
    [Header("Rocket")]
    [SerializeField] GameObject Rocket;
    [SerializeField] float ProjectileSpeed = 0f;
    public float SpawnWait;          // how long the rocket waits when it is red to shoot.
    public float Standby;          
    public float AfterFireStandby = 0;        
    private Animator RocketAnim;
    [Header("Homing Settings")]
    [SerializeField] bool Homing;
    [SerializeField] float HomingRadius;
    [SerializeField] float RocketLifetime = 10f;

    [SerializeField] ParticleSystem SmokeCloud;
    Coroutine CurrentShoot;
    private void Awake()
    {
        RocketAnim = GetComponent<Animator>();
    }
    private IEnumerator RocketWait()
    {
        while(true)
        {
            yield return new WaitForSeconds(Standby);
            RocketAnim.SetTrigger("Prepare");
            yield return new WaitForSeconds(SpawnWait);
            
            RocketAnim.SetTrigger("Shoot");
            SmokeCloud.Play();
            ShootRocket();
            yield return new WaitForSeconds(AfterFireStandby);
        }
    }

    public void ShootRocket()
    {
        ProjectileMotion g_Rocket = Instantiate(Rocket, transform.position + 0.3f*transform.up, transform.rotation).GetComponent<ProjectileMotion>();
        g_Rocket.transform.Rotate(0, 0, 90);
        g_Rocket.lifetime = RocketLifetime;
        // g_Rocket.transform.rotation = Angle;
        g_Rocket.speed = ProjectileSpeed;
        g_Rocket.Homing = Homing;
        g_Rocket.GetComponent<CircleCollider2D>().radius = HomingRadius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            CurrentShoot = StartCoroutine(RocketWait());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (CurrentShoot != null)       // check for other players in range here.
            {
                StopCoroutine(CurrentShoot);
            }
        }
    }
}
