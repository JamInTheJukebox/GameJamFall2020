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
    private float CurrentSpawnWait;


    [SerializeField] bool Homing;
    [SerializeField] float HomingRadius;
    [SerializeField] float LifeTime = 10f;
   
    private void Update()
    {
        CurrentSpawnWait += Time.deltaTime;
        if(CurrentSpawnWait > SpawnWait)
        {
            CurrentSpawnWait = 0;
            ProjectileMotion g_Rocket = Instantiate(Rocket, transform.position, transform.rotation).GetComponent<ProjectileMotion>();
            g_Rocket.transform.Rotate(0, 0, 90);
            g_Rocket.lifetime = LifeTime;
           // g_Rocket.transform.rotation = Angle;
            g_Rocket.speed = ProjectileSpeed;
            g_Rocket.Homing = Homing;
            g_Rocket.GetComponent<CircleCollider2D>().radius = HomingRadius;
        }
    }
}
