using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Slot_Panel_Movement : MonoBehaviour
{
    private int StepLength;
    private float TimeInterval;
    private bool MoveUp = false;
    private Transform StartPosition;
    private Transform EndPosition;
    private GameObject Item;
    private bool SelfSpinning = false;
    private Vector2 InitialPosition;

    private bool FinishedLastLoop;
    private Slot_Machine_Controller MachineController;
    ParticleSystem.MainModule particleSys;

    private void Awake()
    {
        MoveUp = gameObject.name.Contains("2");
        MachineController = FindObjectOfType<Slot_Machine_Controller>();
        Item = transform.Find("Item").gameObject;
        InitialPosition = transform.localPosition;

        SetStep(MachineController.Slot_StepLength);
        TimeInterval = MachineController.TimeInterval;
        SetRange(MachineController.StartPos, MachineController.EndPos);
        if (Item.GetComponent<SpawnVFX_Animator>() != null)
            particleSys = Item.GetComponent<SpawnVFX_Animator>().TheParticle.main;
    }

    private void SetStep(int newSpeed)
    {
        StepLength = newSpeed;
        StepLength *= (MoveUp) ? 1 : -1;
    }

    private void SetRange(Transform StartPos, Transform EndPos)
    {
        StartPosition = (!MoveUp) ? StartPos : EndPos;
        EndPosition = (!MoveUp) ? EndPos: StartPos;
    }

    private void Update()
    {
        if(!SelfSpinning && MachineController.IsSpinning)
        {
            SelfSpinning = true;
            StartCoroutine("TranslatePanel");
        }
        CheckRange();
    }

    private void CheckRange()
    {
        if (MoveUp)
        {
            if (transform.localPosition.y > EndPosition.localPosition.y)
            {
                RestartLoop();
            }
        }
        else
        {
            if (transform.localPosition.y < EndPosition.localPosition.y)
            {
                RestartLoop();                    
            }
        }
    }

    private void RestartLoop()
    {
        Vector3 newPos = transform.localPosition;
        newPos.y = StartPosition.localPosition.y;
        transform.localPosition = newPos;
        SelectRandomItem();
        if (!MachineController.IsSpinning)
        {
            FinishedLastLoop = true;
            if (gameObject.name.Contains("Down"))
                Item.GetComponent<Image>().sprite = MachineController.ChosenItem.Item_PNG;
        }
    }

    public void SelectRandomItem()
    {
        Slot_Item newItem = MachineController.RetreiveRandomItem();
        Item.GetComponent<Image>().sprite = newItem.Item_PNG;
    }

    private IEnumerator TranslatePanel()
    {
        FinishedLastLoop = false;
        while (MachineController.IsSpinning)
        {
            transform.localPosition += new Vector3(0, StepLength, 0);
            yield return new WaitForSeconds(TimeInterval);
        }


        Vector2 CurrentPos = transform.localPosition;
        bool ExitCondition = !FinishedLastLoop;
        while (ExitCondition)
        {
            CurrentPos = transform.localPosition;
            transform.localPosition += new Vector3(0, StepLength, 0);
            yield return new WaitForSeconds(TimeInterval);
            if (FinishedLastLoop)
            {
                ExitCondition = Vector2.Distance(CurrentPos, InitialPosition) > 3;
            }
        }

        transform.localPosition = InitialPosition;
        SelfSpinning = false;
        if(Item.GetComponent<Animator>() != null)
        {
            Item.GetComponent<Animator>().SetTrigger("Explode");
            particleSys.startColor = MachineController.ChosenItem.Pixel_Color;
            // do not create your own material instance
        }
    }

}
