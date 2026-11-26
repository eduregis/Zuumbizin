using UnityEngine;

namespace ZoombieBoy.Core {
    public class LevelManager : MonoBehaviour {
        [Header("References")]
        [SerializeField] private MapData mapData;
        [SerializeField] private Transform levelRoot;
        [SerializeField] private GameObject floorPrefab;
        [SerializeField] private GameObject wallPrefab;
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

            // 2. Calculate offset to center the map
            // If map is 3x3, rows-1 is 2. 2 / 2 = 1. Center is at cell (1,1).
            float centerRow = (mapData.Rows - 1) / 2f;
            float centerCol = (mapData.Columns - 1) / 2f;

            if (levelRoot != null) {
                // O sinal de + no Z compensa o fato de o mapa estar crescendo para trás
                levelRoot.position = new Vector3(-centerCol * cellSize, 0f, centerRow * cellSize);
            }

            // 3. Build the grid
            for (int r = 0; r < mapData.Rows; r++) {
                for (int c = 0; c < mapData.Columns; c++) {
                    TileType tile = mapData.GetTile(r, c);
                    GameObject prefab = tile == TileType.Floor ? floorPrefab : wallPrefab;
                    if (prefab == null) continue;

                    Vector3 pos = CellToWorld(r, c);
                    GameObject instance = Instantiate(prefab, pos, Quaternion.identity, levelRoot);
                    instance.name = $"Tile_{r}_{c}_{tile}";
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
                spawnPos.y += playerYOffset; // Lift the player above the floor

                GameObject playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

                // Optional: Parent the player to LevelRoot so it moves with the map
                playerInstance.transform.SetParent(levelRoot);

                GridMovement movement = playerInstance.GetComponent<GridMovement>();
                if (movement != null) {
                    movement.Setup(this);
                }
            }
        }

        public Vector3 CellToWorld(int row, int col) {
            // Invert the row calculation on the Z axis so Row 0 is at the back
            return new Vector3(col * cellSize, 0f, -(row * cellSize));
        }

        public Vector2Int WorldToCell(Vector3 worldPos) {
            int col = Mathf.RoundToInt(worldPos.x / cellSize);
            // Invert here too so the math matches
            int row = Mathf.RoundToInt(-worldPos.z / cellSize);
            return new Vector2Int(row, col);
        }

        public bool IsWalkable(int row, int col) {
            return mapData != null && mapData.GetTile(row, col) == TileType.Floor;
        }
    }
}