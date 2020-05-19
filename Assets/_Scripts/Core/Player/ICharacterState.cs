using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICharacterState
{
    protected ICharacterStateMachine m_characterStateMachine;

    public ICharacterState(ICharacterStateMachine stateMachine)
    {
        m_characterStateMachine = stateMachine;
    }

    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator Move()
    {
        yield break;
    }
}