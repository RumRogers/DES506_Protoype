﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Please note: although we are going to use icons to display rules in the game, the inner logic is
// still to be defined by a Context-Free Grammar and valid lexemes (terminal symbols) that will
// eventually be bound to actual art assets.
// There's no way outta this :)

namespace GameCore
{
    public class Rule : MonoBehaviour
    {
        // In the following, RuleVerb refers to a (dis)equality sign (=, !=) as decided during design
        // LogicalOperator is (for now) only meant to be a + sign (AND equivalent)

        // Rules have the following Grammar:
        // Rule -> RuleBasic | RuleComplete_1 | RuleComplete_2
        // RuleBasic -> (RuleSubjectOrObject | RuleSubject) RuleVerb (RuleSubjectOrObject | RuleObject)
        // RuleComplete_1 -> RuleBasic LogicalOperator (RuleSubjectOrObject | RuleObject)
        // RuleComplete_2 -> (RuleSubjectOrObject | RuleSubject) LogicalOperator RuleBasic
        // RuleSubject (example) -> not sure, but better have them ready to be used
        // RuleSubjectOrObject (example) -> "Baba", "Keke",
        // RuleObject (example) -> "You", "Win", "Lose"
        // RuleVerb (example) -> "IS", "IS NOT", "BIGGER", "SMALLER"
        // LogicalOperator (example) -> "AND"

        // TODO: we should probably consider more variety? Example > obj or < obj would increase or decrease
        // size of the subject(s) relative to the object

        List<RuleChunk> m_ruleChunks = new List<RuleChunk>();
        // Avoid hardcoding valid positions as they might change during development:
        // much better storing them in a dedicated hashmap.
        // Note that a RuleChunk type can have multiple valid positions within the list...
        Dictionary<RuleChunk.ChunkType, List<uint>> m_validPositions =
            new Dictionary<RuleChunk.ChunkType, List<uint>>
        {
        { RuleChunk.ChunkType.RULE_SUBJECT, new List<uint>{ 0 } },
        { RuleChunk.ChunkType.RULE_SUBJECT_OR_OBJECT, new List<uint>{ 0, 2, 4 } },
        { RuleChunk.ChunkType.RULE_OBJECT, new List<uint>{ 2, 4 } },
        { RuleChunk.ChunkType.RULE_VERB, new List<uint>{ 1, 3 } },
        { RuleChunk.ChunkType.RULE_LOGICAL_OP, new List<uint>{ 1, 3 } },

        };

        const int MIN_RULE_SIZE = 3;
        const int MAX_RULE_SIZE = 5;
        /*public bool p_IsValid
        {
            get
            {
                int ruleLength = m_ruleChunks.Count;

                switch(ruleLength)
                {
                    case MIN_RULE_SIZE:
                        return IsValidRuleBasic(0);
                    case MAX_RULE_SIZE:
                        return (IsValidRuleBasic(0) && ) || (IsValidRuleBasic(2)) &&

                    default:
                        return false;

                }
            }
        }*/
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        // By construction, a RuleBasic can be Head or Tail of RuleComplete.
        // We need to parametrise the control.
        bool IsValidRuleBasic(int startingIdx)
        {
            return ((m_ruleChunks[startingIdx].p_RuleChunkType == RuleChunk.ChunkType.RULE_SUBJECT_OR_OBJECT
                       || m_ruleChunks[startingIdx].p_RuleChunkType == RuleChunk.ChunkType.RULE_SUBJECT)
                       && m_ruleChunks[startingIdx + 1].p_RuleChunkType == RuleChunk.ChunkType.RULE_VERB
                       && (m_ruleChunks[startingIdx + 2].p_RuleChunkType == RuleChunk.ChunkType.RULE_SUBJECT_OR_OBJECT)
                       || m_ruleChunks[startingIdx + 2].p_RuleChunkType == RuleChunk.ChunkType.RULE_OBJECT);
        }

        bool IsValidRuleComplete()
        {
            return false;
        }

        // Pretty prints the rule as in [ Subject Verb Object ]
        public override string ToString()
        {
            string res = "[ ";

            foreach (var chunk in m_ruleChunks)
            {
                res += chunk + " ";
            }

            res += "]";

            return res;
        }
    }

}