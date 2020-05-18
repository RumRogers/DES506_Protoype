using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region INTERFACES (STATE MACHINE AND STATE)
public abstract class ICharacterState
{
    protected ICharacterStateMachine m_characterStateMachine;

    public ICharacterState (ICharacterStateMachine stateMachine)
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

public abstract class ICharacterStateMachine : MonoBehaviour
{
    protected ICharacterState m_state;

    public void SetState(ICharacterState state)
    {
        m_state = state;
        StartCoroutine(m_state.Start());
    }
}
#endregion

#region STATES
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
        m_characterMovment.Direction = new Vector3(Input.GetAxisRaw("Horizontal") ,0.0f , Input.GetAxisRaw("Vertical")).normalized;
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
            if (Input.GetButtonDown("Jump"))
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

#endregion

public class CharacterMovement : ICharacterStateMachine
{
    //Interacting
    [Header("Pushing")]
    [SerializeField] AnimationCurve m_pushMovementCurve = new AnimationCurve();
    [SerializeField] float m_pushingSpeed = 2.0f;
    List<Transform> m_interactablesInRange = new List<Transform>();
    Transform m_closestInteractable = null;

    //Player properties / stats
    CapsuleCollider m_PlayerCollider;
    [Header("Ground Movement")]
    [SerializeField] float m_maxSpeed = 2.0f;
    [SerializeField] float m_walkingAcceleration = 15.0f;
    [SerializeField] float m_walkingDeceleration = 15.0f;
    [SerializeField] float m_maxClimbableIncline = 45.0f;
    [Header("Air Movement")]
    [SerializeField] float m_aerialAccelleration = 5.0f;
    [SerializeField] float m_gravity = 9.81f;
    [SerializeField] float m_jumpVelocity = 4.5f;
    Vector3 m_direction;
    Vector3 m_velocity = Vector3.zero;

    //Hit info
    RaycastHit m_groundedHitInfo;
    RaycastHit m_collisionHitInfo;

    #region PUBLIC ACCESSORS
    //I feel bad about making these one line... but there's going to be a few
    public Vector3 Velocity { get => m_velocity; set => m_velocity = value; }
    public Vector3 Direction { get => m_direction; set => m_direction = value; }
    //player stats, getters only
    public float MaxSpeed { get => m_maxSpeed; }
    public float WalkingAcceleration { get => m_walkingAcceleration; }
    public float WalkingDeceleration { get => m_walkingDeceleration; }
    public float AerialAccelleration { get => m_aerialAccelleration; }
    public float PushSpeed { get => m_pushingSpeed; }
    public float Gravity { get => m_gravity; }
    public float JumpVelocity { get => m_jumpVelocity; }
    //Pushing Getters
    public Transform GetClosestInteractable { get => m_closestInteractable; }
    public AnimationCurve GetPushMovementCurve { get => m_pushMovementCurve; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerCollider = transform.GetComponent<CapsuleCollider>();
        SetState(new DefaultState(this));
        m_pushMovementCurve.postWrapMode = WrapMode.PingPong;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(m_state.Move());

        if (Colliding())
            m_velocity = new Vector3(0.0f, m_velocity.y, 0.0f);


        if (Input.GetButtonDown("Submit"))
        {
            switch (m_state)
            {
                case DefaultState d:
                    //if in default state, get the closest movable to the player and change state to the pushing state 
                    //(List could be gotten in the state itself with an accessor, but encapsulation is a thing we like)
                    if (m_interactablesInRange.Count > 0)
                    {
                        float closestDistance = (m_interactablesInRange[0].position - transform.position).magnitude;
                        m_closestInteractable = m_interactablesInRange[0];
                        for (int i = 1; i < m_interactablesInRange.Count; ++i)
                        {
                            float distance = (m_interactablesInRange[i].position - transform.position).magnitude;
                            if (distance < closestDistance)
                            {
                                m_closestInteractable = m_interactablesInRange[i];
                                closestDistance = distance;
                            }
                        }
                        SetState(new PushingState(this));
                    }                    
                    break;
                case PushingState p:
                    SetState(new DefaultState(this));
                    break;
                default:
                    break;
            }
        }

        //Adding velocity to position
        transform.position += m_velocity * Time.deltaTime;
    }

    void Climb()
    {
        Vector3 perpVecToSlope = Vector3.Cross(transform.right, m_groundedHitInfo.normal);
        Debug.DrawLine(transform.position, transform.position + perpVecToSlope);
        m_velocity.y = perpVecToSlope.y * m_velocity.magnitude;
    }

    bool Climbable()
    {
        Vector3 perpVecToSlope = Vector3.Cross(transform.right, m_collisionHitInfo.normal);
        float angleBetween = Vector3.Angle(perpVecToSlope, new Vector3(m_velocity.x, 0, m_velocity.z));        
        if (Mathf.Abs(angleBetween) > m_maxClimbableIncline)
        {
            return false;
        }    
        return true;
    }

    bool Colliding()
    {
        return Physics.Raycast(new Vector3( transform.position.x, transform.position.y - (m_PlayerCollider.bounds.extents.y - 0.05f), transform.position.z), new Vector3(m_velocity.x, 0.0f, m_velocity.z).normalized, out m_collisionHitInfo, m_PlayerCollider.bounds.extents.x);
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, out m_groundedHitInfo, m_PlayerCollider.bounds.extents.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Movable")
        {
            m_interactablesInRange.Add(other.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Movable")
        {
            foreach (Transform t in m_interactablesInRange)
            {
                if (t.GetInstanceID() == other.transform.GetInstanceID())
                {
                    if (t == m_closestInteractable && m_state.GetType() == typeof(PushingState))
                    {
                        SetState(new DefaultState(this));
                    }
                    m_interactablesInRange.Remove(t);
                    return;
                }
            }
        }
    }
}
