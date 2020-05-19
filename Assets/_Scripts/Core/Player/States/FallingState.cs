using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : ICharacterState
{
    Vector3 m_Velocity = Vector3.zero;
    CharacterMovement m_characterMovment;

    public FallingState(CharacterMovement stateMachine) : base(stateMachine)
    {
        m_characterMovment = stateMachine;
        m_Velocity = m_characterMovment.Velocity;
    }

    public override IEnumerator Start()
    {
        yield return null;
    }

    public override IEnumerator Move()
    {
        m_characterMovment.Velocity += (Vector3.down * m_characterMovment.Gravity) * Time.deltaTime;

        //if grounded transition to default state
        if (m_characterMovment.IsGrounded())
        {
            m_characterMovment.Velocity = new Vector3(m_characterMovment.Velocity.x, 0, m_characterMovment.Velocity.z);
            m_characterMovment.SetState(new DefaultState(m_characterMovment));
        }

        //Directional input, slower in mid air
        m_characterMovment.Direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
        if (m_characterMovment.Direction != Vector3.zero)
        {
            m_Velocity += (m_characterMovment.Direction * m_characterMovment.AerialAccelleration) * Time.deltaTime;
            m_Velocity = Vector3.ClampMagnitude(m_Velocity, m_characterMovment.MaxSpeed);
        }

        m_characterMovment.Velocity = new Vector3(m_Velocity.x, m_characterMovment.Velocity.y, m_Velocity.z);
        yield return null;
    }
}
