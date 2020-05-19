using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICharacterStateMachine : MonoBehaviour
{
    protected ICharacterState m_state;

    public void SetState(ICharacterState state)
    {
        m_state = state;
        StartCoroutine(m_state.Start());
    }
}
