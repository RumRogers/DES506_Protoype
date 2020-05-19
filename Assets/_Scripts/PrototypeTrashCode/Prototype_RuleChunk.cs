using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype_RuleChunk : MonoBehaviour
{
    public enum RuleChunkType
    {
        EMPTY,
        SUBJECT_PLAYER,
        VERB_IS,
        VERB_IS_NOT,
        VERB_CAN,
        VERB_CAN_NOT,
        OBJECT_DROWN,
        OBJECT_JUMP
    }

    [SerializeField]
    RuleChunkType m_ruleChunkType;
    public RuleChunkType RuleChunk { get { return m_ruleChunkType; } private set { m_ruleChunkType = value; } }
    Prototype_LevelRules m_levelRules;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            m_levelRules = GameObject.FindGameObjectWithTag("LevelRules").GetComponent<Prototype_LevelRules>();
        }
        catch(UnityException ex)
        {
            print(ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Movable"))
        {
            try
            {
                Prototype_RuleChunk othersRuleChunk = other.GetComponentInParent<Prototype_RuleChunk>();
                switch(m_ruleChunkType)
                {
                    case RuleChunkType.SUBJECT_PLAYER:
                        if(othersRuleChunk.m_ruleChunkType >= RuleChunkType.VERB_IS && othersRuleChunk.m_ruleChunkType <= RuleChunkType.VERB_CAN_NOT)
                        {
                            print("Rule changed...");
                            m_levelRules.m_slot_2 = othersRuleChunk.m_ruleChunkType;
                        }
                        break;
                    case RuleChunkType.VERB_IS:
                    case RuleChunkType.VERB_IS_NOT:
                    case RuleChunkType.VERB_CAN:
                    case RuleChunkType.VERB_CAN_NOT:
                        if (othersRuleChunk.m_ruleChunkType >= RuleChunkType.OBJECT_DROWN && othersRuleChunk.m_ruleChunkType <= RuleChunkType.OBJECT_JUMP)
                        {
                            if(m_levelRules.m_slot_2 != RuleChunkType.EMPTY)
                            {
                                print("Rule complete...");
                                m_levelRules.m_slot_3 = othersRuleChunk.m_ruleChunkType;
                            }
                            
                        }
                        break;
                }
            }
            catch(UnityException whatever)
            {
                print("You messed up!");
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Movable"))
        {
            try
            {
                var othersChunkType = other.GetComponentInParent<Prototype_RuleChunk>().m_ruleChunkType;
                if(!(m_levelRules.IsObject(m_ruleChunkType) && m_levelRules.IsObject(othersChunkType)))
                {
                    // Undo rule only if separated chunks aren't both Objects
                    m_levelRules.UndoRule(m_ruleChunkType, other.GetComponentInParent<Prototype_RuleChunk>().m_ruleChunkType);
                }
                

            }
            catch (UnityException ex)
            {
                print(ex.Message);
            }
        }
    }
}
