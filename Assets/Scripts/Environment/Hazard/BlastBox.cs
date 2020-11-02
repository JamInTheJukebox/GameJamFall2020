using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastBox : MonoBehaviour
{

    private int HashBlast;
    private Animator BlastAnim;
    public GameObject Blast_Hazard;
    private BoxCollider2D BlastCol;
    private bool ReadyToExplode;

    private void Awake()
    {
        BlastAnim = GetComponent<Animator>();
        HashBlast = Animator.StringToHash("BlastBox_activate");
        BlastCol = GetComponent<BoxCollider2D>();
    }

    public void DestroyPlatform()
    {
        ReadyToExplode = true;
        BlastCol.enabled = false;
        Blast_Hazard.SetActive(true);
    }

    public void Respawn()
    {
        ReadyToExplode = false;
        BlastCol.enabled = true;        // you can easily make it so this code is called by the respawn animation
        Blast_Hazard.SetActive(false);
    }

    private void OnEnable()
    {
        Respawn();
    }
    

    public void InitiateDestruction()
    {
        if(!ReadyToExplode)
            BlastAnim.Play(HashBlast);
    }
}
