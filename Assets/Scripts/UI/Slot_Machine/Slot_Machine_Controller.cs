using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Slot_Machine_Controller : MonoBehaviour
{
    public Animator SlotAnimator;
    [Header("Slot Panels")]
    public int Slot_StepLength = 1;                             // slots move at a range from -125 to 125. Therefore, this integer must be a factor of 250.
    public float TimeInterval = 0.01f;                          // the amount of time a slot waits before moving again
    private int ProbabalisticSum;                               // holds the sum of all the probabilities
    public List<Slot_Item> Items = new List<Slot_Item>();       // a list that dictates all possible events.
    public Transform StartPos;                                  // Start pos and end pos are meant for teleporting a slot panel when it is out of view to give the illusion of looping                     
    public Transform EndPos;                                    
    public bool IsSpinning = true;                              // A bool that the slot panel movement.cs script will get to make sure it is allowed to move.

    [Header("Time")]
    public Color OriginalColor;
    public Color SpinColor;
    [Tooltip("The amount of time the slot is idle before it spins again.")]
    [SerializeField] int Total_Idle_Time;
    private string m_String_Idle_Time;
    public string String_Idle_Time
    {
        get
        {
            return m_String_Idle_Time;
        }
        set
        {
            if(m_String_Idle_Time != value)
            {
                m_String_Idle_Time = value;
            }
        }
    }
    [SerializeField] float TotalSpinTime;


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
    public TextMeshProUGUI TimerText;
    private float m_TimeElapsed;
    public float TimeElapsed
    {
        get
        {
            return m_TimeElapsed;
        }
        protected set
        {
            m_TimeElapsed = value;
            UpdateTime();
            
        }
    }

    private bool m_InFrame;
    public bool InFrame
    {
        get { return m_InFrame; }
        set {
            if(m_InFrame == value) { return; }
            m_InFrame = value;
            SlotAnimator.SetBool("InFrame", m_InFrame); }
    }
    private void UpdateTime()
    {
        if (IsSpinning) { return; }
        string minutes, seconds;
        ConvertTimetoString(out minutes, out seconds);
        if (TimerText.text == minutes + ":" + seconds) { return; }
        if (minutes + seconds == String_Idle_Time)
        {
            InFrame = true;
            TimerText.text = String_Idle_Time[0] + ":" + String_Idle_Time[1] + String_Idle_Time[2];
            Invoke("SpinSlotMachine",0.5f);
            return;
        }
        TimerText.text = minutes + ":" + seconds;
        float LerpRate = TimeElapsed / Total_Idle_Time;
        TimerText.color = Color.Lerp(TimerText.color, SpinColor, LerpRate);
    }

    private void SpinSlotMachine()
    {
        IsSpinning = true;
        TimeElapsed = 0;
        // add other effects here
    }
    public void StopSlotMachine()
    {
        InFrame = false;
        TimerText.color = OriginalColor;
        TimeElapsed = 0;
    }

    private void ConvertTimetoString(out string minutes, out string seconds)
    {
        minutes = Mathf.Floor(m_TimeElapsed / 60).ToString("0");
        seconds = (m_TimeElapsed % 60).ToString("00");
        if (seconds == "60")
        {
            seconds = "00"; minutes = Mathf.Ceil(m_TimeElapsed / 60).ToString("0");
        }
    }

    private void Awake()
    {
        string minutes = Mathf.Floor(Total_Idle_Time / 60).ToString("0");
        string seconds = (Total_Idle_Time % 60).ToString("00");
        String_Idle_Time = minutes + seconds;
    }

    private void Update()
    {
        if(!IsSpinning && !InFrame)
            TimeElapsed += Time.deltaTime;
        else if(IsSpinning)
        {
            TimeElapsed += Time.deltaTime;
            if(TimeElapsed >= TotalSpinTime)
            {
                ChosenItem = DefineProbabilityTree();
                //print("Chosen Item: " + ChosenItem);
                IsSpinning = false;
                // play an animation here
                //Invoke("StopSlotMachine", 2f);      // amount of time the animation takes.
                // call a function to call a shader on the item
                //Move back to original position when the shader is done.
            }
        }
    }


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

        return Items[Index];
    }

    public Slot_Item RetreiveRandomItem()
    {
        int index = Random.Range(0, Items.Count);
        return Items[index];
    }

    public void RemoveItem(Slot_Item item)
    {
        Items.Remove(item);
    }
}

