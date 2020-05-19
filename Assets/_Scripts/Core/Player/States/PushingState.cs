using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingState : ICharacterState
{
    const float GRID_SIZE = 2.0f;
    bool m_moving = false;
    CharacterMovement m_characterMovement;

    public PushingState(CharacterMovement stateMachine) : base(stateMachine)
    {
        m_characterMovement = stateMachine;
        m_characterMovement.Velocity = Vector3.zero;
    }

    public override IEnumerator Start()
    {
        yield return null;
    }

    public override IEnumerator Move()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            if (!m_moving)
            {
                m_moving = true;
                m_characterMovement.Direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
                Vector3 finalPosition = m_characterMovement.transform.position + (m_characterMovement.Direction * GRID_SIZE);
                Vector3 currentPosition = m_characterMovement.transform.position;
                Vector3 movableOffset = m_characterMovement.GetClosestInteractable.position - m_characterMovement.transform.position;
                float time = 0;
                float timeToLerp = GRID_SIZE / m_characterMovement.PushSpeed;
                while (true)
                {
                    time += Time.deltaTime;
                    float perComp = time / timeToLerp;
                    float curvedPerComp = m_characterMovement.GetPushMovementCurve.Evaluate(perComp);

                    if (perComp > 0.99)
                    {
                        break;
                    }
                    if (m_characterMovement.GetClosestInteractable != null)
                        m_characterMovement.GetClosestInteractable.position = Vector3.Lerp(currentPosition + movableOffset, finalPosition + movableOffset, curvedPerComp);

                    m_characterMovement.transform.position = Vector3.Lerp(currentPosition, finalPosition, curvedPerComp);
                    yield return null;
                }
                m_characterMovement.transform.position = finalPosition;
                m_moving = false;
            }
        }
        yield return null;
    }

}
