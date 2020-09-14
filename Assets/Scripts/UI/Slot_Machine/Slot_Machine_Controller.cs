using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot_Machine_Controller : MonoBehaviour
{
    public int Slot_StepLength = 1;
    public float TimeInterval = 0.01f;
    private int ProbabalisticSum;
    public List<Slot_Item> Items = new List<Slot_Item>();
    public Transform[] Slot_Panels;
    public Transform StartPos;
    public Transform EndPos;
    public bool IsSpinning = true;

    private Slot_Item m_ChosenItem;
    public Slot_Item ChosenItem
    {
        get
        {
            return m_ChosenItem;
        }
        protected set
        {
            if (m_ChosenItem != value)
                m_ChosenItem = value;
        }
    }

    private void Start()
    {
        foreach(Transform Panel in Slot_Panels)
        {
            if(Panel.GetComponent<Slot_Panel_Movement>() == null)
            {
                Panel.gameObject.AddComponent<Slot_Panel_Movement>();
                Debug.LogWarning("Slot_Machine_Controller.cs: You are missing required components for the slot machine to work properly.");
            }

            Slot_Panel_Movement PanelMovement = Panel.GetComponent<Slot_Panel_Movement>();

            PanelMovement.SetStep(Slot_StepLength);
            PanelMovement.SetTimeInterval(TimeInterval);
            PanelMovement.SetRange(StartPos, EndPos);
        }
        Invoke("DefineProbabilityTree", 4f);
    }

    private void Update()
    {
        float Y_dif = Slot_Panels[0].position.y - Slot_Panels[3].position.y;
        //print(Mathf.Abs(Y_dif));
    }
    /// <summary>
    /// 
    /// 
    /// 10
    /// 12
    /// 15
    /// 20
    /// 30
    /// 50
    /// </summary>
    /// <returns></returns>

    private Slot_Item DefineProbabilityTree()                // define a Cumulative Distribution Function. Each step is determined by the probabalistic factor found in each slot_item
    {
        int Sum = 0;
        int[] Probabilities = new int[Items.Count];
        for(int i = 0; i < Items.Count; i++)
        {
            int CurrentProb = Items[i].Probability;
            Sum += CurrentProb;
            if (i-1 >= 0)
            {
                CurrentProb += Probabilities[i - 1];
            }
            Probabilities[i] = CurrentProb;
        }

        int Choice = Random.Range(0, Sum);
        int Index = 0;
        for (int i = 0; i < Probabilities.Length; i++)
        {
            int Lower = 0;
            if(i != 0)
            {
                Lower = Probabilities[i - 1];
            }

            if (Choice >= Lower && Choice < Probabilities[i])
            {
                Index = i;
                break;
            }
        }

        ChosenItem = Items[Index];
        print(ChosenItem.name);
        return Items[Index];
    }

    public Slot_Item RetreiveRandomItem()
    {
        int index = Random.Range(0, Items.Count);
        return Items[index];
    }
}

