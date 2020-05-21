using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.TileSystem
{
    public class GroundTile : AbstractTile
    {
        protected override void SetType()
        {
            m_type = TileType.GROUND;
        }

        protected override void ApplyEffect()
        {
            
        }
    }
}