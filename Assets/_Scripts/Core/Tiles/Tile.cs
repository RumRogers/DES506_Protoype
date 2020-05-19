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
        [SerializeField]
        float m_tileSize = 2.0f;

        //PlayerEffect m_playerEffect;
        //tmpBasicMovement m_tmpBasicMovement;
        CharacterMovement m_characterMovement;

        /*public void SetPlayerState()
        {
            if(m_playerEffect == PlayerEffect.DEAD)
            {
                m_tmpBasicMovement.m_playerState = tmpBasicMovement.PlayerState.DEAD;
            }
            if (m_playerEffect == PlayerEffect.NOT_DEAD)
            {
                m_tmpBasicMovement.m_playerState = tmpBasicMovement.PlayerState.ALIVE;
            }
        }*/

        public void Awake()
        {
            try
            {
                m_characterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
            }
            catch(UnityException ex)
            {
                print(ex.Message);
            }

            //GameObject player = GameObject.FindGameObjectWithTag("Player");

            /*if (player != null)
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
            }*/

        }

        /*private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SetPlayerState();
                Debug.Log("State is " + m_tmpBasicMovement.m_playerState);
            }
        }*/

        private void Update()
        {
           if(IsPlayerInside())
            {
                //print("Player in tile of type " + m_tileType);
                if(m_tileType == TileType.WATER && m_characterMovement.CanDrown && !m_characterMovement.Drowning)
                {
                    m_characterMovement.Drowning = true;
                    print("Player now drowning!!!");
                }
            }

        }

        bool IsPlayerInside()
        {
            Vector3 pos = m_characterMovement.transform.position;

            return (pos.x >= transform.position.x - m_tileSize / 2) &&
                    (pos.x <= transform.position.x + m_tileSize / 2) &&
                    (pos.z <= transform.position.z + m_tileSize / 2) &&
                    (pos.z <= transform.position.z - m_tileSize / 2);
        }
    }
}