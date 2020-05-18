using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public abstract class RuleChunk
    {
        public enum ChunkType
        {
            RULE_SUBJECT, // Strict subject
            RULE_VERB,
            RULE_OBJECT, // Strict object
            RULE_SUBJECT_OR_OBJECT, // interchangeable
            RULE_LOGICAL_OP
        };

        public RuleChunk(ChunkType chunkType, string lexeme)
        {
            p_RuleChunkType = chunkType;
            m_lexeme = lexeme;
        }
        [SerializeField]
        public ChunkType p_RuleChunkType { get; private set; }
        // Lexeme can be any string that's valid for the concrete RuleChunk (+, =, !=, NAME)
        protected string m_lexeme;

        public override string ToString()
        {
            return m_lexeme;
        }
    }

}