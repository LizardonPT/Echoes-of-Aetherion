using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EchoesOfAetherion.CameraUtils
{
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private GameObject cameraPivot;
        [SerializeField] private Transform[] targets;
        private Vector3 velocity = Vector3.zero;

        [SerializeField, Range(0, .3f)] private float smoothTime = 0.05f;
        [SerializeField] private Vector3 positionOffset;

        [Header("Limitation")]
        [SerializeField] private bool hasLimits;
        [SerializeField] private Vector2 xLimit;
        [SerializeField] private Vector2 yLimit;

#if UNITY_EDITOR
        [SerializeField] private Color limitsDebugColour = Color.blue;
# endif

        private Camera cam;

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        private void FixedUpdate()
        {
            if (targets.Length > 0)
            {
                Vector3 targetPosition;
                if (targets.Length == 1)
                {
                    targetPosition = targets[0].position + positionOffset;
                }
                else
                {
                    Vector3 midpoint = Vector3.zero;
                    foreach (var target in targets)
                    {
                        midpoint += target.position;
                    }
                    midpoint /= targets.Length;
                    targetPosition = midpoint + positionOffset;
                }

                if (hasLimits)
                {
                    Vector3 clampedPosition = ClampPosition(targetPosition);
                    SmoothMove(clampedPosition);
                }
                else
                {
                    SmoothMove(targetPosition);
                }
            }
        }

        public void SetTarget(Transform target)
        {
            targets = new Transform[] { target };
            cameraPivot.transform.position = target.position + positionOffset;
        }

        public void SetTargets(Transform[] targets)
        {
            this.targets = targets;
        }

        public void AddTarget(Transform target)
        {
            List<Transform> targetList = new List<Transform>(targets);
            if (!targetList.Contains(target))
            {
                targetList.Add(target);
            }
            targets = targetList.ToArray();
        }

        public void RemoveTarget(Transform target)
        {
            List<Transform> targetList = new List<Transform>(targets);
            if (targetList.Contains(target))
            {
                targetList.Remove(target);
            }
            targets = targetList.ToArray();
        }

        public void SetHasLimits(bool b)
        {
            hasLimits = b;
        }

        private Vector3 ClampPosition(Vector3 targetPosition)
        {
            float clampedX = Mathf.Clamp(targetPosition.x, xLimit.x, xLimit.y);
            float clampedY = Mathf.Clamp(targetPosition.y, yLimit.x, yLimit.y);

            return new Vector2(clampedX, clampedY);
        }

        private void SmoothMove(Vector3 targetPosition)
        {
            cameraPivot.transform.position =
                Vector3.SmoothDamp(cameraPivot.transform.position, targetPosition, ref velocity, smoothTime);
        }

        public void ChangeCameraSize(float targetSize, float duration)
        {
            StartCoroutine(ChangeCameraSizeCoroutine(targetSize, duration));
        }

        private IEnumerator ChangeCameraSizeCoroutine(float targetSize, float duration)
        {
            float startSize = cam.orthographicSize;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                cam.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            cam.orthographicSize = targetSize;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!hasLimits) return;
            if (cam == null)
            {
                cam = GetComponent<Camera>();
                if (cam == null) return;
            }

            Gizmos.color = limitsDebugColour;

            float camHeight = cam.orthographicSize * 2;
            float camWidth = camHeight * cam.aspect;

            Vector3 center = new Vector3((xLimit.x + xLimit.y) / 2, (yLimit.x + yLimit.y) / 2, cameraPivot.transform.position.z);
            Vector3 size = new Vector3(xLimit.y - xLimit.x + camWidth, yLimit.y - yLimit.x + camHeight, 1);

            Gizmos.DrawWireCube(center, size);
        }
#endif
        private void OnValidate()
        {
            if (cameraPivot == null)
            {
                cameraPivot = transform.parent != null ? transform.parent.gameObject : null;
            }
        }
    }
}
