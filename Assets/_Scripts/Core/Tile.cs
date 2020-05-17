using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class Tile : Automaton
    {
        enum TileType
        {

        };

        [SerializeField]
        protected float row { get => row; set => row = value; }
        [SerializeField]
        protected float col { get => col; set => col = value; }
        [SerializeField]
        protected uint plane { get => plane; set => plane = value; }

        protected State state;

        // Start is called before the first frame update
        void Start()
        {

        }

        protected override void Update()
        {
            base.Update(); // Call the Automaton update
        }
    }
}