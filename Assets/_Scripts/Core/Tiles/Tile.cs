using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class Tile : MonoBehaviour
    {
        enum TileType
        { 
            GRASS,
            WATER
        };

        enum PlayerEffect
        {
            NOT_DEAD,
            DEAD
        };

        [SerializeField]
        protected float row { get => row; set => row = value; }
        [SerializeField]
        protected float col { get => col; set => col = value; }
        [SerializeField]
        protected uint plane { get => plane; set => plane = value; }
        [SerializeField]
        TileType m_tileType;

        PlayerEffect m_playerEffect;
        tmpBasicMovement m_tmpBasicMovement;

        public void SetPlayerState()
        {
            if(m_playerEffect == PlayerEffect.DEAD)
            {
                m_tmpBasicMovement.m_playerState = tmpBasicMovement.PlayerState.DEAD;
            }
            if (m_playerEffect == PlayerEffect.NOT_DEAD)
            {
                m_tmpBasicMovement.m_playerState = tmpBasicMovement.PlayerState.ALIVE;
            }
        }

        public void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                m_tmpBasicMovement = player.GetComponent<tmpBasicMovement>();
            }

            if(m_tileType == TileType.WATER)
            {
                m_playerEffect = PlayerEffect.DEAD;
            }
            if (m_tileType == TileType.GRASS)
            {
                m_playerEffect = PlayerEffect.NOT_DEAD;
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SetPlayerState();
                Debug.Log("State is " + m_tmpBasicMovement.m_playerState);
            }
        }
    }
}