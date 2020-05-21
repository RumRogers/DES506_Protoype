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
        //Adds velocity based on entity property flags
        if (m_characterMovment.HasProperty(EntityProperties.JUMP_NORMAL))
            m_characterMovment.Velocity = new Vector3(m_characterMovment.Velocity.x, m_characterMovment.JumpVelocity, m_characterMovment.Velocity.z);

        if (m_characterMovment.HasProperty(EntityProperties.JUMP_HIGH)) 
            m_characterMovment.Velocity = new Vector3(m_characterMovment.Velocity.x, m_characterMovment.HighJumpVelocity, m_characterMovment.Velocity.z);

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
        Vector3 forwardMovement = new Vector3( Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z )* Input.GetAxisRaw("Vertical");
        Vector3 rightMovement = Camera.main.transform.right * Input.GetAxisRaw("Horizontal");
        m_characterMovment.Direction = forwardMovement + rightMovement;
        m_characterMovment.Direction = m_characterMovment.Direction.normalized;

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