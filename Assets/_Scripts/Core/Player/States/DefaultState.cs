using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultState : ICharacterState
{
    Vector3 m_Velocity = Vector3.zero;

    CharacterMovement m_characterMovment;

    public DefaultState(CharacterMovement stateMachine) : base(stateMachine)
    {
        m_characterMovment = stateMachine;
    }

    public override IEnumerator Start()
    {
        yield return null;
    }

    public override IEnumerator Move()
    {
        //Directional input
        m_characterMovment.Direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
        if (m_characterMovment.Direction != Vector3.zero)
        {
            m_Velocity += (m_characterMovment.Direction * m_characterMovment.WalkingAcceleration) * Time.deltaTime;
            m_Velocity = Vector3.ClampMagnitude(m_Velocity, m_characterMovment.MaxSpeed);
        }
        else if (Mathf.Abs(m_Velocity.x) > 0.3f || Mathf.Abs(m_Velocity.z) > 0.3f)
        {
            m_Velocity += (((m_Velocity.normalized) * -1) * m_characterMovment.WalkingDeceleration) * Time.deltaTime;
        }
        else
        {
            m_Velocity.x = 0.0f;
            m_Velocity.z = 0.0f;
        }

        m_characterMovment.Velocity = new Vector3(m_Velocity.x, m_characterMovment.Velocity.y, m_Velocity.z);

        //Jumping / falling state transition
        if (m_characterMovment.IsGrounded())
        {
            if (Input.GetButtonDown("Jump") && m_characterMovment.CanJump)
            {
                m_characterMovment.SetState(new JumpingState(m_characterMovment));
            }
        }
        else
        {
            m_characterMovment.SetState(new FallingState(m_characterMovment));
        }
        yield return null;
    }
}
