using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class RuleSubject : RuleChunk
    {
        public RuleSubject(RuleChunk.ChunkType chunkType, string lexeme) : base(chunkType, lexeme)
        { }
    }
}

