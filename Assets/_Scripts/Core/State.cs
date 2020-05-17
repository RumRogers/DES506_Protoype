using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    // Automaton and State implement the State pattern: we avoid changing behavior with long switch cases
    // by wrapping each case in its class.

    public abstract class State
    {
        private Automaton m_owner;

        /// <summary>
        /// Sets this State's owner through dependency injection
        /// </summary>
        /// <param name="owner">The Automaton instance that uses this State</param>
        public State(Automaton owner)
        {
            m_owner = owner;
        }

        /// <summary>
        /// Each concrete State needs to implement this method with ad-hoc behavior
        /// </summary>
        public abstract void Manage();
    }
}