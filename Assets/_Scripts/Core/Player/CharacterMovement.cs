using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityProperties
{
    MOVEABLE            = 1 << 1,
    PLAYABLE            = 2 << 1,
    DROWNING            = 3 << 1,
    CAN_JUMP            = 4 << 1,
    JUMP_NORMAL         = 5 << 1,
    JUMP_HIGH           = 6 << 1,
    CAN_DROWN           = 7 << 1
}

public class CharacterMovement : ICharacterStateMachine
{
    //Interacting
    [Header("Pushing")]
    [SerializeField] AnimationCurve m_pushMovementCurve = new AnimationCurve();
    [SerializeField] float m_pushingSpeed = 2.0f;
    List<Transform> m_interactablesInRange = new List<Transform>();
    Transform m_closestInteractable = null;

    //Player properties / stats
    CapsuleCollider m_PlayerCollider;   //TODO: Change the capsule collider to a box collider or generic collider as not every shape is going to be a capsule
    [Header("Ground Movement")]
    [SerializeField] float m_maxSpeed = 2.0f;
    [SerializeField] float m_walkingAcceleration = 15.0f;
    [SerializeField] float m_walkingDeceleration = 15.0f;
    [Header("Air Movement")]
    [SerializeField] float m_aerialAccelleration = 5.0f;
    [SerializeField] float m_gravity = 9.81f;
    [SerializeField] float m_jumpVelocity = 4.5f;
    [SerializeField] float m_highJumpVelocity = 9.5f;
    [Header("Collision")]
    [SerializeField] float m_maxClimbableIncline = 45.0f;
    [SerializeField] float m_groundPadding = 0.1f;  //How far from the floor the ray should start
    [SerializeField] float m_collisionRayLengthMultiplyer = 0.7f;   //Determines what percentage of the player's bounds to use as ray length
    [SerializeField] float m_groundOverlapPadding = 0.1f;
    [Header("Properties (Debug)")]
    [SerializeField] bool m_drowning = false;
    [SerializeField] bool m_canJump = true;
    [SerializeField] bool m_canDrown = false;
    Vector3 m_direction;
    Vector3 m_velocity = Vector3.zero;
    Vector3 m_startPosition;

    //Hit info
    RaycastHit m_groundedHitInfo;
    RaycastHit m_collisionHitInfo;

    EntityProperties m_entityProperties;

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
    public float HighJumpVelocity { get => m_highJumpVelocity; }
    public float MaxClimableAngle { get => m_maxClimbableIncline; }
    public float GroundOverlapPadding { get => m_groundOverlapPadding; }
    public CapsuleCollider PlayerCollider { get => m_PlayerCollider; }
    public Vector3 StartPosition { get => m_startPosition; }
    public RaycastHit GroundHitInfo { get => m_groundedHitInfo; }
    public EntityProperties EntityProperties { get => m_entityProperties; }

    //temp
    public bool Drowning { get => m_drowning; set => m_drowning = value; }
    public bool CanDrown { get => m_canDrown; set => m_canDrown = value; }
    public bool CanJump { get => m_canJump; set => m_canJump = value; }
    //Pushing Getters
    public Transform GetClosestInteractable { get => m_closestInteractable; }
    public AnimationCurve GetPushMovementCurve { get => m_pushMovementCurve; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        AddEntityProperty(EntityProperties.CAN_JUMP);
        AddEntityProperty(EntityProperties.JUMP_NORMAL);

        m_startPosition = transform.position;
        m_PlayerCollider = transform.GetComponent<CapsuleCollider>();
        SetState(new DefaultState(this));
        m_pushMovementCurve.postWrapMode = WrapMode.Clamp;
    }

    // Update is called once per frame
    void Update()
    {
        if (Drowning)
            SetState(new DeathState(this));

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

    bool Colliding()
    {
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - (m_PlayerCollider.bounds.extents.y - m_groundPadding), transform.position.z),
            new Vector3(m_velocity.x, 0.0f, m_velocity.z).normalized, out m_collisionHitInfo, m_PlayerCollider.bounds.extents.x * m_collisionRayLengthMultiplyer))
        {
            return m_collisionHitInfo.collider.isTrigger ? false : true;    //if collider is a trigger, ignore it and return false
        }
        return false;        
    }

    public bool IsGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out m_groundedHitInfo, m_PlayerCollider.bounds.extents.y))
        {
            return m_groundedHitInfo.collider.isTrigger ? false : true; //if stood on trigger, ignore it and return false
        }
        return false;
    }

    //CHANGING / CHECKING PROPERTIES
    public void ClearEntityProperties()
    {
        m_entityProperties = 0;
    }

    public void AddEntityProperty(EntityProperties property)
    {
        m_entityProperties |= property;
    }

    public void RemoveEntityProperty(EntityProperties property)
    {
        m_entityProperties &= ~property;
    }

    public void ReplaceEntityProperty(EntityProperties toRemove, EntityProperties toAdd)
    {
        m_entityProperties &= ~toRemove;
        m_entityProperties |= toAdd;
    }

    public bool HasProperty(EntityProperties property)
    {
        return (m_entityProperties & property) == property;
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
