using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZoombieBoy.Core {
    public class GridMovement : MonoBehaviour {
        private LevelManager levelManager;
        [SerializeField] private float stepDuration = 0.18f;

        private bool isMoving;
        private Vector2Int pendingDirection = Vector2Int.zero;

        // Called by LevelManager right after instantiating the player
        public void Setup(LevelManager manager) {
            levelManager = manager;
        }

        public void OnMove(InputValue value) {
            if (isMoving || levelManager == null) return;

            Vector2 input = value.Get<Vector2>();
            Vector2Int dir = Vector2Int.zero;

            // Left / Right
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                dir = new Vector2Int(0, input.x > 0 ? 1 : -1);
            // Up / Down
            else if (Mathf.Abs(input.y) > 0.1f)
                dir = new Vector2Int(input.y > 0 ? -1 : 1, 0); // W (-1) e S (+1)

            if (dir == Vector2Int.zero) return;

            pendingDirection = dir;
            TryStep();
        }

        private void TryStep() {
            if (isMoving || pendingDirection == Vector2Int.zero) return;

            Vector2Int current = levelManager.WorldToCell(transform.position);
            Vector2Int target = current + pendingDirection;

            if (!levelManager.IsWalkable(target.x, target.y)) return;

            StartCoroutine(StepRoutine(current, target));
        }

        private IEnumerator StepRoutine(Vector2Int from, Vector2Int to) {
            isMoving = true;

            Vector3 startPos = levelManager.CellToWorld(from.x, from.y);
            Vector3 endPos = levelManager.CellToWorld(to.x, to.y);

            float t = 0f;
            while (t < 1f) {
                t += Time.deltaTime / stepDuration;
                transform.position = Vector3.Lerp(startPos, endPos, Mathf.Clamp01(t));
                yield return null;
            }

            transform.position = endPos;
            isMoving = false;
        }
    }
}