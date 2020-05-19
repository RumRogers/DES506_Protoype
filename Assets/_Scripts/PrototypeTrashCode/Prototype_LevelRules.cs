using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype_LevelRules : MonoBehaviour
{
    public Prototype_RuleChunk.RuleChunkType m_slot_1 = Prototype_RuleChunk.RuleChunkType.SUBJECT_PLAYER;
    public Prototype_RuleChunk.RuleChunkType m_slot_2 = Prototype_RuleChunk.RuleChunkType.EMPTY;
    public Prototype_RuleChunk.RuleChunkType m_slot_3 = Prototype_RuleChunk.RuleChunkType.EMPTY;

    [SerializeField]
    bool m_applied = false;
    CharacterMovement m_player;
    private void Start()
    {
        try
        {
            m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        }
        catch(UnityException ex)
        {
            print(ex.Message);
        }
    }

    private void Update()
    {
        if(m_slot_2 != Prototype_RuleChunk.RuleChunkType.EMPTY && m_slot_3 != Prototype_RuleChunk.RuleChunkType.EMPTY && !m_applied)
        {            
            ApplyRule();
        }
    }
    public void ApplyRule()
    {
        m_applied = true;

        if(m_slot_2 == Prototype_RuleChunk.RuleChunkType.VERB_CAN)
        {
            print("Applying Rule: " + this);
            switch (m_slot_3)
            {
                case Prototype_RuleChunk.RuleChunkType.OBJECT_JUMP:
                    m_player.CanJump = true;
                    break;
                case Prototype_RuleChunk.RuleChunkType.OBJECT_DROWN:
                    m_player.CanDrown = true;                    
                    break;
                default:
                    throw new UnityException("Unrecognized object rule chunk!");
            }   
        }
    }

    public void UndoRule(Prototype_RuleChunk.RuleChunkType chunk1, Prototype_RuleChunk.RuleChunkType chunk2)
    {
        Prototype_RuleChunk.RuleChunkType sortedChunk_1 = chunk1 < chunk2 ? chunk1 : chunk2;
        Prototype_RuleChunk.RuleChunkType sortedChunk_2 = sortedChunk_1 == chunk1 ? chunk2 : chunk1;
        
        if(sortedChunk_1 < Prototype_RuleChunk.RuleChunkType.VERB_IS)
        {
            m_slot_2 = Prototype_RuleChunk.RuleChunkType.EMPTY;
        }
        else
        {
            m_slot_3 = Prototype_RuleChunk.RuleChunkType.EMPTY;
        }

        m_applied = false;

        // We're undoing a CAN/CANNOT rule, so the new predicate is the opposite of the previous
        bool predicate = sortedChunk_1 == Prototype_RuleChunk.RuleChunkType.VERB_CAN ? false : true;
        if (sortedChunk_2 == Prototype_RuleChunk.RuleChunkType.OBJECT_JUMP)
        {
            m_player.CanJump = predicate;
        }
        else if (sortedChunk_2 == Prototype_RuleChunk.RuleChunkType.OBJECT_DROWN)
        {
            m_player.CanDrown = predicate;
        }

        print("Undo Rule: " + this + ", moved " + chunk1 + " away from " + chunk2);
    }

    void ResetState()
    {
        m_slot_2 = Prototype_RuleChunk.RuleChunkType.EMPTY;
        m_slot_3 = Prototype_RuleChunk.RuleChunkType.EMPTY;
        m_applied = false;
    }

    public override string ToString()
    {
        return m_slot_1 + " " + m_slot_2 + " " + m_slot_3;
    }

    public bool IsSubject(Prototype_RuleChunk.RuleChunkType type)
    {
        return type == Prototype_RuleChunk.RuleChunkType.SUBJECT_PLAYER;
    }

    public bool IsVerb(Prototype_RuleChunk.RuleChunkType type)
    {
        return type >= Prototype_RuleChunk.RuleChunkType.VERB_IS && type <= Prototype_RuleChunk.RuleChunkType.VERB_CAN_NOT;
    }

    public bool IsObject(Prototype_RuleChunk.RuleChunkType type)
    {
        return type >= Prototype_RuleChunk.RuleChunkType.OBJECT_DROWN;
    }
}
