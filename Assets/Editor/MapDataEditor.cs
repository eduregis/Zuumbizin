using UnityEditor;
using UnityEngine;
using ZoombieBoy.Core;

[CustomEditor(typeof(MapData))]
public class MapDataEditor : Editor {
    public override void OnInspectorGUI() {
        MapData map = (MapData)target;

        EditorGUILayout.LabelField("Map Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Dimension controls
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Row")) {
            Undo.RecordObject(map, "Add Row");
            map.AddRow();
            EditorUtility.SetDirty(map);
            AssetDatabase.SaveAssets(); // Force save to disk
        }
        GUI.enabled = map.Rows > 1;
        if (GUILayout.Button("Remove Row")) {
            Undo.RecordObject(map, "Remove Row");
            map.RemoveRow();
            EditorUtility.SetDirty(map);
            AssetDatabase.SaveAssets();
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Column")) {
            Undo.RecordObject(map, "Add Column");
            map.AddColumn();
            EditorUtility.SetDirty(map);
            AssetDatabase.SaveAssets();
        }
        GUI.enabled = map.Columns > 1;
        if (GUILayout.Button("Remove Column")) {
            Undo.RecordObject(map, "Remove Column");
            map.RemoveColumn();
            EditorUtility.SetDirty(map);
            AssetDatabase.SaveAssets();
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (map.Rows == 0 || map.Columns == 0) {
            EditorGUILayout.HelpBox("Add a row to start building the map.", MessageType.Info);
            return;
        }

        // Grid rendering
        for (int r = 0; r < map.Rows; r++) {
            EditorGUILayout.BeginHorizontal();
            for (int c = 0; c < map.Columns; c++) {
                bool isSpawn = map.SpawnCell.x == r && map.SpawnCell.y == c;
                GUIStyle spawnStyle = new GUIStyle(GUI.skin.button);
                if (isSpawn) spawnStyle.normal.textColor = Color.green;

                if (GUILayout.Button(isSpawn ? "S" : "", spawnStyle, GUILayout.Width(20))) {
                    Undo.RecordObject(map, "Set Spawn");
                    map.SetSpawn(r, c);
                    EditorUtility.SetDirty(map);
                    AssetDatabase.SaveAssets();
                }

                TileType current = map.GetTile(r, c);
                bool isFloor = current == TileType.Floor;
                bool newValue = EditorGUILayout.Toggle(isFloor, GUILayout.Width(20));
                if (newValue != isFloor) {
                    Undo.RecordObject(map, "Change Tile");
                    map.SetTile(r, c, newValue ? TileType.Floor : TileType.Wall);
                    EditorUtility.SetDirty(map);
                    AssetDatabase.SaveAssets();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Dimensions: {map.Rows} rows x {map.Columns} cols", EditorStyles.miniLabel);
        EditorGUILayout.LabelField($"Spawn: (row {map.SpawnCell.x}, col {map.SpawnCell.y})", EditorStyles.miniLabel);
    }
}