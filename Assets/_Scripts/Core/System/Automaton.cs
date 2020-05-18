using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    // Automaton and State implement the State pattern: we avoid changing behavior with long switch cases
    // by wrapping each case in its class.

    // Each class that can change its behavior according to its specific state, should inherit from Automaton
    public abstract class Automaton : MonoBehaviour
    {
        // The current State the Automaton is in
        protected State m_state = null;

        // This should ideally be called by new State instances that pass "this" as argument
        public void SetState(State state)
        {
            m_state = state;
        }

        // Overridable by specialised classes
        protected virtual void Update()
        {
            if(m_state != null)
            {
                m_state.Manage();
            }
        }
    }
}


