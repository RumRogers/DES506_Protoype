using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Tiles
{
    // A tile type is implicit in its hierarchy. This is the basic tile.
    public class Tile_Refactor : MonoBehaviour
    {
        [SerializeField]
        public static float s_TILE_SIZE = 1.0f;

        [SerializeField]
        private int m_row;
        [SerializeField]
        public int m_col;
        [SerializeField]
        protected int m_layer;
        [SerializeField]
        protected Tiles.TileCommon.TileType m_type;


        private void Start()
        {
            m_type = TileCommon.TileType.GROUND;
        }

        // Basic tiles don't affect the environment. Specialized tiles should override this.
        public virtual void ApplyEffect() { }

        private string FormatRowColLayerTriple()
        {
            return "[Row: " + m_row + ", Col: " + m_col + ", Layer: " + m_layer + "]";
        }

        public override string ToString()
        {
            return "BasicTile, " + FormatRowColLayerTriple();
        }
    }
}