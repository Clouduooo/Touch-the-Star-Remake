using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum JumpInput
{
    Up, Down, Left, Right, None
}

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }  //Only read outside the class, write inside
    public JumpInput jumpDir;

    private void Start()
    {
        jumpDir = JumpInput.None;
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void JumpInputUp(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            jumpDir = JumpInput.Up;
        }
        //if(context.canceled)
        //{
        //    jumpDir = JumpInput.None;
        //}
    }
    public void JumpInputDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpDir = JumpInput.Down;
        }
        //if (context.canceled)
        //{
        //    jumpDir = JumpInput.None;
        //}
    }
    public void JumpInputLeft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpDir = JumpInput.Left;
        }
        //if (context.canceled)
        //{
        //    jumpDir = JumpInput.None;
        //}
    }
    public void JumpInputRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpDir = JumpInput.Right;
        }
        //if (context.canceled)
        //{
        //    jumpDir = JumpInput.None;
        //}
    }
}
