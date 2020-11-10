using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public enum e_ProjectileType { Rocket = 0, Mine = 0};
    [Header("Rocket")]
    public GameObject Rocket;
    [SerializeField] float ProjectileSpeed;
    public float SpawnWait;
    public float Standby;
    private Animator RocketAnim;
    //private SpriteRenderer RocketSprite;

    [SerializeField] bool Homing;
    [SerializeField] float HomingRadius;
    [SerializeField] float LifeTime = 10f;

    public ParticleSystem SmokeCloud;

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
        }
    }

    public void ShootRocket()
    {
        ProjectileMotion g_Rocket = Instantiate(Rocket, transform.position + new Vector3(0,0.3f,0), transform.rotation).GetComponent<ProjectileMotion>();
        g_Rocket.transform.Rotate(0, 0, 90);
        g_Rocket.lifetime = LifeTime;
        // g_Rocket.transform.rotation = Angle;
        g_Rocket.speed = ProjectileSpeed;
        g_Rocket.Homing = Homing;
        g_Rocket.GetComponent<CircleCollider2D>().radius = HomingRadius;
    }

    private void OnEnable()
    {
        StartCoroutine(RocketWait());
    }
}
