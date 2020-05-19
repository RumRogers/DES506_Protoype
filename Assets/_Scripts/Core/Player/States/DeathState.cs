using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeathState : ICharacterState
{
    CharacterMovement m_characterMovment;

    public DeathState(CharacterMovement stateMachine) : base(stateMachine)
    {
        Debug.Log("In DeathState!");
        m_characterMovment = stateMachine;
        m_characterMovment.transform.position = m_characterMovment.StartPosition;
        //m_characterMovment.Drowning = false;
        //m_characterMovment.SetState(new DefaultState(m_characterMovment));
    }

    public override IEnumerator Start()
    {
        yield return null;
    }

    public override IEnumerator Move()
    {
        yield return null;
    }
}
