using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Levels
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private int m_gridRows;
        [SerializeField]
        private int m_gridCols;
        // This should be filled in at Start() with every game entity that can be affected by the rules
        //private List<MutableEntity> m_mutableEntities;

        // This maps each floor number to its actual LevelFloor
        // Whoever manipulates the level and creates new floors, must *manually* assign it a proper
        // y position: e.g. GroundFloor has Y == 0, FirstFloor should have Y == 1,
        // UndergroundFloor should have Y == -1 etc.
        private Dictionary<int, List<Tiles.Tile_Refactor>> m_tilesLayers = new Dictionary<int, List<Tiles.Tile_Refactor>>();

        // Start is called before the first frame update
        void Start()
        {
            Debug.Assert(SetupTiles(), "Could not initialize level tiles!");
        }

        // Update is called once per frame
        void Update()
        {

        }

        bool SetupTiles()
        {
            List<GameObject> floors = RetrieveLevelFloors();

            // Each level needs at least one floor
            if(floors.Count == 0)
            {
                return false;
            }

            foreach(var floor in floors)
            {
                // Each floor will have an integer number so beware of this
                int floorNum = (int)floor.transform.position.y;

                InitGridOnFloor(floorNum);

                List<GameObject> floorTiles = RetrieveTilesGameObjectsFromFloor(floor);

                foreach(var tile in floorTiles)
                {
                    int col = (int)((transform.position.x + m_gridCols) + tile.transform.position.x);
                    int row = (int)((transform.position.z + m_gridRows) - tile.transform.position.z);

                    //m_tilesLayers[floorNum][m_gridCols * row + col] = new Tiles.Tile_Refactor()
                    Debug.Log(tile.transform.position);
                    print("col = " + col + ", row = " + row);                    
                }

                
            }

            return true;
        }

        bool InitGridOnFloor(int floorNum)
        {
            // First and only time to add the floor number as new key to the dictionary
            m_tilesLayers[floorNum] = new List<Tiles.Tile_Refactor>();

            if (!m_tilesLayers.ContainsKey(floorNum))
            {
                return false;
            }

            int n = (int)m_gridRows * (int)m_gridCols;
            m_tilesLayers[floorNum] = new List<Tiles.Tile_Refactor>();

            for(int i = 0; i < n; ++i)
            {
                m_tilesLayers[floorNum].Add(null);
            }

            return true;
        }

        List<GameObject> RetrieveLevelFloors()
        {
            return new List<GameObject>(GameObject.FindGameObjectsWithTag("Floor"));
        }

        List<GameObject> RetrieveTilesGameObjectsFromFloor(GameObject floor)
        {
            List<GameObject> tiles = new List<GameObject>();

            foreach(Transform t in floor.transform)
            {
                if(t.CompareTag("Tile"))
                {
                    tiles.Add(t.gameObject);
                }
            }

            return tiles;
        }
    }
}

