using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public PlayerControls Controls;
    /*
    static bool Pressed_Lasso;          // lasso arc, or C
    static bool Pressed_Lasso_Dir;      // lasso direction, or v
    static bool Pressed_Jump;           // space
    static bool Pressed_Run;            // left shift
    */
    public float Horizontal;            // AD
    public float Vertical;              // WS
    public bool Running;                // Left Shift
    public bool Jumping;                // Space

    private void Awake()
    {
        Controls = new PlayerControls();

        Controls.Player.Horizontal.performed += ctx => { Horizontal = ctx.ReadValue<float>(); };
        Controls.Player.Vertical.performed += ctx => { Vertical = ctx.ReadValue<float>(); };

        Controls.Player.Horizontal.canceled += ctx => { Horizontal = 0; };
        Controls.Player.Vertical.canceled += ctx => { Vertical = 0; };

        //dControls.Player. += ctx => { Vertical = 0; };
        Controls.Player.Run.started += ctx => { Running = true; };
        Controls.Player.Run.canceled += ctx => { Running = false; };

        Controls.Player.Jump.performed += ctx => { Jumping = true; };
        Controls.Player.Jump.canceled += ctx => { Jumping = false; };

    }

    public bool JumpTriggered()
    {
        return Controls.Player.Jump_T.triggered;
    }

    public bool RunTriggered()
    {
        return Controls.Player.Run_T.triggered;
    }

    public bool LassoTriggered()
    {
        return Controls.Player.Lasso_T.triggered;
    }

    public bool LassoDirTriggered()
    {
        return Controls.Player.LassoDir_T.triggered;
    }
    public bool RunStopTriggered()
    {
        return Controls.Player.Run_TR.triggered;
    }
    public bool PauseTriggered()
    {
        return Controls.Player.Pause.triggered;
    }
    public bool Lasso_Y_Triggered()
    {
        return Controls.Player.LassoDirY.triggered;
    }
    private void OnEnable()
    {
        Controls.Player.Enable();
    }

    private void OnDisable()
    {
        Controls.Player.Disable();
    }


}
