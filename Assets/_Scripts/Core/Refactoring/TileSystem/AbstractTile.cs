using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.TileSystem
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class AbstractTile : MonoBehaviour
    {
        public enum TileType
        {
            GROUND, WATER
        }

        // Implicitly static
        public const float s_TILE_SIZE = 1.0f;

        [SerializeField]
        // Only needed for coherency in the editor, so it's visually obvious what type the tile is
        protected TileType m_type;
        private BoxCollider m_triggerBoxCollider;
        // The GameObject that is currently contained within this tile
        protected GameObject m_currentGameObject;

        private void Start()
        {
            SetType();
            Vector3 truncatedPosition = transform.position;
            truncatedPosition.x = Mathf.RoundToInt(truncatedPosition.x);
            truncatedPosition.y = Mathf.RoundToInt(truncatedPosition.y);
            truncatedPosition.z = Mathf.RoundToInt(truncatedPosition.z);
            transform.position = truncatedPosition;

            m_triggerBoxCollider = gameObject.AddComponent<BoxCollider>() as BoxCollider;
            m_triggerBoxCollider.size.Set(s_TILE_SIZE, s_TILE_SIZE, s_TILE_SIZE);
            m_triggerBoxCollider.center += s_TILE_SIZE * Vector3.up;
        }


        // Each concrete type will have to set its own type by implementing this.
        // It is useless for the logic but will help visually during level creation
        protected abstract void SetType();

        // This is the real deal of the tile system, as each concrete tile will need to specify what happens
        // when a game entity steps on it.
        // Please note that "no effect" is just an implementation with an empty method body.
        protected abstract void ApplyEffect();

        protected void OnTriggerEnter(Collider other)
        {
            m_currentGameObject = other.gameObject;
            ApplyEffect();
        }

        protected void OnTriggerExit(Collider other)
        {
            // This might not be true when having a TriggerEnter and TriggerExit at the same time
            // (think of boxes stacked horizontally and going through this tile)
            if(other.gameObject == m_currentGameObject)
            {
                m_currentGameObject = null;
            }
        }
    }
}