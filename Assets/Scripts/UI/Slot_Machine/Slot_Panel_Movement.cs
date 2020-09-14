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

    private void Awake()
    {
        MoveUp = gameObject.name.Contains("2");
        MachineController = FindObjectOfType<Slot_Machine_Controller>();
        Item = transform.Find("Item").gameObject;
        InitialPosition = transform.localPosition;
    }
    public void SetStep(int newSpeed)
    {
        StepLength = newSpeed;
        StepLength *= (MoveUp) ? 1 : -1;
    }

    public void SetTimeInterval(float newScrollSpeed)
    {
        TimeInterval = newScrollSpeed;
    }

    public void SetRange(Transform StartPos, Transform EndPos)
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
            if (transform.position.y > EndPosition.position.y)
            {
                RestartLoop();
            }
        }
        else
        {
            if (transform.position.y < EndPosition.position.y)
            {
                RestartLoop();                    
            }
        }
    }

    private void RestartLoop()
    {
        Vector3 newPos = transform.position;
        newPos.y = StartPosition.position.y;
        transform.position = newPos;
        Slot_Item newItem = MachineController.RetreiveRandomItem();
        Item.GetComponent<Image>().sprite = newItem.Item_PNG;
        if (!MachineController.IsSpinning)
        {
            FinishedLastLoop = true;
            if (gameObject.name.Contains("Down"))
                Item.GetComponent<Image>().sprite = MachineController.ChosenItem.Item_PNG;
        }
    }

    private IEnumerator TranslatePanel()
    {
        FinishedLastLoop = false;
        while (MachineController.IsSpinning)
        {
            transform.position += new Vector3(0, StepLength, 0);
            yield return new WaitForSeconds(TimeInterval);
        }


        Vector2 CurrentPos = transform.localPosition;
        bool ExitCondition = !FinishedLastLoop;
        while (ExitCondition)
        {
            CurrentPos = transform.localPosition;
            transform.position += new Vector3(0, StepLength, 0);
            yield return new WaitForSeconds(TimeInterval);
            if (FinishedLastLoop)
            {
                ExitCondition = Vector2.Distance(CurrentPos, InitialPosition) > 3;
            }

        }

        transform.localPosition = InitialPosition;
        SelfSpinning = false;   
    }

}
