using System.Collections.Generic;
using UnityEngine;

namespace ZoombieBoy.Core {
    public enum TileType {
        Floor,
        Wall
    }

    [CreateAssetMenu(fileName = "NewMap", menuName = "Puzzle/Map Data")]
    public class MapData : ScriptableObject {
        [SerializeField]
        private List<TileType> grid = new List<TileType>();

        [SerializeField]
        private int rows = 0;

        [SerializeField]
        private int columns = 0;

        [Header("Player Spawn")]
        [SerializeField]
        private Vector2Int spawnCell = Vector2Int.zero;

        public int Rows => rows;
        public int Columns => columns;
        public Vector2Int SpawnCell => spawnCell;

        public TileType GetTile(int row, int col) {
            if (row < 0 || row >= rows || col < 0 || col >= columns)
                return TileType.Wall;

            int index = (row * columns) + col;
            return grid[index];
        }

        public void SetTile(int row, int col, TileType type) {
            if (row < 0 || row >= rows || col < 0 || col >= columns) return;

            int index = (row * columns) + col;
            grid[index] = type;
        }

        public void SetSpawn(int row, int col) {
            if (row < 0 || row >= rows || col < 0 || col >= columns) return;
            if (GetTile(row, col) != TileType.Floor) return;
            spawnCell = new Vector2Int(row, col);
        }

        public void AddRow() {
            int newCols = Mathf.Max(columns, 1);
            if (columns == 0) columns = newCols; // First row initializes columns

            for (int c = 0; c < columns; c++) {
                grid.Add(TileType.Floor);
            }
            rows++;
        }

        public void RemoveRow() {
            if (rows <= 1) return;

            // Remove the last 'columns' items
            grid.RemoveRange(grid.Count - columns, columns);
            rows--;
            ClampSpawn();
        }

        public void AddColumn() {
            if (rows == 0) {
                AddRow();
                return;
            }

            // Insert a new floor tile at the end of each row
            for (int r = rows - 1; r >= 0; r--) {
                int insertIndex = (r * columns) + columns; // End of current row
                grid.Insert(insertIndex, TileType.Floor);
            }
            columns++;
        }

        public void RemoveColumn() {
            if (columns <= 1) return;

            // Remove the last element of each row
            for (int r = rows - 1; r >= 0; r--) {
                int removeIndex = (r * columns) + (columns - 1);
                grid.RemoveAt(removeIndex);
            }
            columns--;
            ClampSpawn();
        }

        private void ClampSpawn() {
            int r = Mathf.Clamp(spawnCell.x, 0, Mathf.Max(0, rows - 1));
            int c = Mathf.Clamp(spawnCell.y, 0, Mathf.Max(0, columns - 1));
            spawnCell = new Vector2Int(r, c);
        }
    }
}