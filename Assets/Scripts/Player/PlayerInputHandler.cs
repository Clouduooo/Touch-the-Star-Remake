using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }  //�ⲿֻ�������ڲ���д

    public void MoveInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void JumpInput(InputAction.CallbackContext context)
    {

    }
}
