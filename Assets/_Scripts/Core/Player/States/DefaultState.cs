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
        //Jumping / falling state transition
        if (m_characterMovment.IsGrounded())
        {
            if (Input.GetButtonDown("Jump") && m_characterMovment.CanJump)
            {
                m_characterMovment.SetState(new JumpingState(m_characterMovment));
                yield break;
            }
        }
        else
        {
            m_characterMovment.SetState(new FallingState(m_characterMovment));
            yield break;
        }
        //Directional input
        Vector3 forwardMovement = Camera.main.transform.forward * Input.GetAxisRaw("Vertical");
        Vector3 rightMovement = Camera.main.transform.right * Input.GetAxisRaw("Horizontal");
        m_characterMovment.Direction = forwardMovement + rightMovement;
        m_characterMovment.Direction = m_characterMovment.Direction.normalized;

        //Slope detection 
        //if the slope is climable, modify the direction the player is traveling in 
        if (Vector3.Angle(m_characterMovment.GroundHitInfo.normal, m_characterMovment.transform.up) < m_characterMovment.MaxClimableAngle)
        {
            //get the players right based on direction of movement, then use it to calculate the new direction of travel
            Vector3 playerRight = Vector3.Cross(m_characterMovment.Direction, -m_characterMovment.transform.up);
            //getting the slope angle for the ground the player is walking on
            Vector3 slopeDirection = Vector3.Cross(playerRight, m_characterMovment.GroundHitInfo.normal);

            m_characterMovment.Direction = slopeDirection;
        }

        //Overlap recovery (ground)
        if (m_characterMovment.GroundHitInfo.distance < (m_characterMovment.PlayerCollider.height / 2) - m_characterMovment.GroundOverlapPadding)
        {
            float lerpedY = Mathf.Lerp(m_characterMovment.transform.position.y, m_characterMovment.transform.position.y + 1, Time.deltaTime);
            m_characterMovment.transform.position = new Vector3(m_characterMovment.transform.position.x, lerpedY, m_characterMovment.transform.position.z);
        }

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

        m_characterMovment.Velocity = m_Velocity;

        yield return null;
    }
}
