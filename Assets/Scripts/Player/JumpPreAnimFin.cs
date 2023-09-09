using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPreAnimFin : MonoBehaviour
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
}
