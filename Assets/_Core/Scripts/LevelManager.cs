using UnityEngine;

namespace ZoombieBoy.Core {
    public class LevelManager : MonoBehaviour {
        [Header("References")]
        [SerializeField] private MapData mapData;
        [SerializeField] private Transform levelRoot;
        [SerializeField] private GameObject floorPrefab; // Square 2D com rotação 90 no X
        [SerializeField] private GameObject wallPrefab;  // Square 2D com Billboard
        [SerializeField] private GameObject playerPrefab;

        [Header("Grid")]
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float playerYOffset = 0.05f;

        public MapData MapData => mapData;
        public float CellSize => cellSize;

        private void Start() {
            BuildLevel();
        }

        public void BuildLevel() {
            if (mapData == null) return;

            // 1. Clear old tiles
            if (levelRoot != null) {
                for (int i = levelRoot.childCount - 1; i >= 0; i--)
                    Destroy(levelRoot.GetChild(i).gameObject);
            }

            // 2. Center the map
            float centerRow = (mapData.Rows - 1) / 2f;
            float centerCol = (mapData.Columns - 1) / 2f;

            if (levelRoot != null) {
                levelRoot.position = new Vector3(-centerCol * cellSize, 0f, centerRow * cellSize);
            }

            // 3. Build the grid
            for (int r = 0; r < mapData.Rows; r++) {
                for (int c = 0; c < mapData.Columns; c++) {
                    Vector3 pos = CellToWorld(r, c);
                    TileType tile = mapData.GetTile(r, c);

                    // Always instantiate the floor first
                    if (floorPrefab != null) {
                        GameObject floorInstance = Instantiate(floorPrefab, pos, floorPrefab.transform.rotation, levelRoot);
                        floorInstance.name = $"Floor_{r}_{c}";
                    }

                    // If it's a wall, instantiate the wall on top
                    if (tile == TileType.Wall && wallPrefab != null) {
                        GameObject wallInstance = Instantiate(wallPrefab, pos, Quaternion.identity, levelRoot);
                        wallInstance.name = $"Wall_{r}_{c}";
                    }
                }
            }

            // 4. Spawn the Player
            if (playerPrefab != null) {
                Vector2Int spawn = mapData.SpawnCell;

                if (!IsWalkable(spawn.x, spawn.y)) {
                    Debug.LogWarning("LevelManager: Spawn cell is not walkable, defaulting to (0,0).");
                    spawn = Vector2Int.zero;
                }

                Vector3 spawnPos = CellToWorld(spawn.x, spawn.y);
                spawnPos.y += playerYOffset;

                GameObject playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                playerInstance.transform.SetParent(levelRoot);

                GridMovement movement = playerInstance.GetComponent<GridMovement>();
                if (movement != null) {
                    movement.Setup(this);
                }
            }
        }

        public Vector3 CellToWorld(int row, int col) {
            return new Vector3(col * cellSize, 0f, -(row * cellSize));
        }

        public Vector2Int WorldToCell(Vector3 worldPos) {
            int col = Mathf.RoundToInt(worldPos.x / cellSize);
            int row = Mathf.RoundToInt(-worldPos.z / cellSize);
            return new Vector2Int(row, col);
        }

        public bool IsWalkable(int row, int col) {
            return mapData != null && mapData.GetTile(row, col) == TileType.Floor;
        }
    }
}