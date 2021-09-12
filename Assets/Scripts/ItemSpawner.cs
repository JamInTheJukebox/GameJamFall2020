using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject Item;
    public float RespawnTime;

    private void Awake()
    {
        SpawnItem();
    }

    public void respawnItem()
    {
        StartCoroutine(RespawnTheItem());
    }
    public IEnumerator RespawnTheItem()
    {
        yield return new WaitForSeconds(RespawnTime);
        SpawnItem();
    }

    public void SpawnItem()
    {
        var item = Instantiate(Item, transform.position, Quaternion.identity);
        item.GetComponent<MovementModifier>().SetRespawner(this);
    }
}
