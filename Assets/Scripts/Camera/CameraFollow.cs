using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace EchoesOfAetherion.CameraUtils
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private List<Transform> targets = new();
        [SerializeField, Range(0, .3f)] private float smoothTime = 0.05f;
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private bool hasLimits;
        [SerializeField] private Vector2 xLimit, yLimit;

        private Camera cam;
        private Vector3 velocity;

        private Transform MoveTarget => cameraPivot != null ? cameraPivot : transform;

        private void Awake() => cam = GetComponent<Camera>();

        private void FixedUpdate() => UpdateCameraPosition();

        private void UpdateCameraPosition()
        {
            if (targets.Count == 0) return;

            Vector3 targetPos = (targets.Count == 1)
                ? targets[0].position
                : GetTargetsMidpoint();

            targetPos += positionOffset;

            SmoothMove(targetPos);
            
            if (hasLimits)
                MoveTarget.position = ClampPosition(targetPos);
        }

        private Vector3 GetTargetsMidpoint()
        {
            Vector3 sum = Vector3.zero;
            foreach (var t in targets) sum += t.position;
            return sum / targets.Count;
        }

        private void SmoothMove(Vector3 targetPos)
        {
            MoveTarget.position = Vector3.SmoothDamp(MoveTarget.position, targetPos, ref velocity, smoothTime);
        }

        private Vector3 ClampPosition(Vector3 targetPosition)
        {
            float camHeight = cam.orthographicSize * 2f;
            float camWidth = camHeight * cam.aspect;

            float minX = xLimit.x + camWidth / 2f;
            float maxX = xLimit.y - camWidth / 2f;
            float minY = yLimit.x + camHeight / 2f;
            float maxY = yLimit.y - camHeight / 2f;

            float clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);
            float clampedY = Mathf.Clamp(targetPosition.y, minY, maxY);

            return new Vector3(clampedX, clampedY, targetPosition.z);
        }


        public void SetHasLimits(bool value) => hasLimits = value;

        public void SetTarget(Transform target)
        {
            targets.Clear();
            if (target != null)
                targets.Add(target);
        }

        public void AddTarget(Transform target)
        {
            if (target != null && !targets.Contains(target))
                targets.Add(target);
        }

        public void RemoveTarget(Transform target)
        {
            if (target != null && targets.Contains(target))
                targets.Remove(target);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!hasLimits) return;

            // Ensure we have a camera reference
            if (cam == null)
                cam = GetComponent<Camera>();
            if (cam == null || cameraPivot == null)
                return;

            Gizmos.color = Color.blue;

            // Compute camera visible area
            float camHeight = cam.orthographicSize * 2f;
            float camWidth = camHeight * cam.aspect;

            // Compute the visible bounds of the camera (world-space)
            // These represent the total world area the camera can *move within*
            float minX = xLimit.x + camWidth / 2f;
            float maxX = xLimit.y - camWidth / 2f;
            float minY = yLimit.x + camHeight / 2f;
            float maxY = yLimit.y - camHeight / 2f;

            // Draw a rectangle showing the camera movement area
            Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, cameraPivot.transform.position.z);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, 1f);

            // Draw wireframe of visible area the camera can travel through
            Gizmos.DrawWireCube(center, size);

            // Optionally, draw the actual world bounds (where camera cannot see past)
            Gizmos.color = new Color(0f, 0.3f, 1f, 0.25f);

            Vector3 worldCenter = new Vector3(
                (xLimit.x + xLimit.y) / 2f,
                (yLimit.x + yLimit.y) / 2f,
                cameraPivot.transform.position.z
            );
            Vector3 worldSize = new Vector3(
                xLimit.y - xLimit.x,
                yLimit.y - yLimit.x,
                1f
            );
            Gizmos.DrawWireCube(worldCenter, worldSize);
        }
#endif

    }

}
