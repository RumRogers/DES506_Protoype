using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class tmpBasicMovement : MonoBehaviour
{
    private float speed = 2.0f;
    //public GameObject character;



    public enum PlayerState
    {
        ALIVE,
        DEAD
    };
    
    public PlayerState m_playerState { get; set; }

    void Update()
    {

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += Vector3.back * speed * Time.deltaTime;
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Tile"))
    //    {
    //        other.gameObject.GetComponent<GameCore.Tile>().SetPlayerState();
    //        Debug.Log(m_playerState);
    //    }
    //}
}
