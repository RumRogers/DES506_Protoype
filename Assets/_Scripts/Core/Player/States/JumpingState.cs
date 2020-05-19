using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : ICharacterState
{
    Vector3 m_Velocity = Vector3.zero;
    CharacterMovement m_characterMovment;

    public JumpingState(CharacterMovement stateMachine) : base(stateMachine)
    {
        m_characterMovment = stateMachine;
        m_characterMovment.Velocity = new Vector3(m_characterMovment.Velocity.x, m_characterMovment.JumpVelocity, m_characterMovment.Velocity.z);
        m_Velocity = m_characterMovment.Velocity;
    }

    public override IEnumerator Start()
    {
        yield return null;
    }

    public override IEnumerator Move()
    {
        //subtracting gravity from upwards velocity until forces equalise
        m_characterMovment.Velocity += (Vector3.down * m_characterMovment.Gravity) * Time.deltaTime;

        //Directional input, slower in mid air
        m_characterMovment.Direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
        if (m_characterMovment.Direction != Vector3.zero)
        {
            m_Velocity += (m_characterMovment.Direction * m_characterMovment.AerialAccelleration) * Time.deltaTime;
            m_Velocity = Vector3.ClampMagnitude(m_Velocity, m_characterMovment.MaxSpeed);
        }

        m_characterMovment.Velocity = new Vector3(m_Velocity.x, m_characterMovment.Velocity.y, m_Velocity.z);

        if (m_characterMovment.Velocity.y < 0)
        {
            m_characterMovment.SetState(new FallingState(m_characterMovment));
        }
        yield return null;
    }

}