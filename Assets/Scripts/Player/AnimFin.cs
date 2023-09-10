using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimFin : MonoBehaviour
{
    PlayerFSM playerFSM;
    void Awake()
    {
        playerFSM=FindObjectOfType<PlayerFSM>();
    }

    public void JumpPrepareAnimFin()
    {
        playerFSM.parameter.isJumpPreAnimFin=true;
    }

    public void JumpRollingAnimFin()
    {
        playerFSM.parameter.isJumpRollingAnimFin=true;
    }
}
