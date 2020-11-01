using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpawnVFX_Animator : MonoBehaviour
{
    public ParticleSystem TheParticle;
    public Sprite NullImage;
    private void Awake()
    {
        if (TheParticle != null)
            TheParticle.Stop();
    }
    public void OnAnimTrigger()
    {
        //Instantiate(TheParticle, transform.position, Quaternion.identity);
        GetComponent<Image>().sprite = NullImage;
        TheParticle.Play();
        Invoke("Set_Out_Of_Frame",0.2f);
        
    }

    public void BurstParticle()
    {
        if(TheParticle != null)
        {
            TheParticle.Play();
        }
    }
    private void Set_Out_Of_Frame()
    {
        FindObjectOfType<Slot_Machine_Controller>().StopSlotMachine();
        Invoke("SetItem", 3f);
    }

    private void SetItem()
    {
        transform.parent.GetComponent<Slot_Panel_Movement>().SelectRandomItem();
    }
}
