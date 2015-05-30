using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PlaneCamera
{
    [SelectionBase]
    public class Camera : MonoBehaviour
    {
        [Header("Reorientation Speed")]
        public float focusTime = .25f;
        public float pivotTime = .25f;
        public float depthTime = .25f;

        [Header("Orientation Limits")]
        public float minPivot =  0;
        public float maxPivot = 90;
        public float minDepth =  5;
        public float maxDepth = 15;

        public Rect worldBounds;

        [Header("Link Pivot & Zoom")]
        public bool linkPivotToDepth;
        public AnimationCurve pivotZoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Target Orientation")]
        public Vector2 focusTarget;
        public float pivotTarget;
        public float depthTarget;

        [Header("Internal Transforms")]
        [SerializeField]
        protected Transform depthTransform;
        [SerializeField]
        protected Transform pivotTransform;
        [SerializeField]
        protected Transform focusTransform;

        public Vector2 focus
        {
            get
            {
                return new Vector2(focusTransform.position.x,
                                   focusTransform.position.z);
            }

            protected set
            {
                focusTransform.position = new Vector3(value.x, 0, value.y);
            }
        }

        public float pivot
        {
            get
            {
                return pivotTransform.localEulerAngles.x;
            }

            protected set
            {
                pivotTransform.localEulerAngles = Vector3.right * value;
            }
        }

        public float depth
        {
            get
            {
                return -depthTransform.localPosition.z;
            }

            protected set
            {
                depthTransform.localPosition = Vector3.back * value;
            }
        }

        private Vector2 focusVelocity;
        private float pivotVelocity;
        private float depthVelocity;

        protected virtual void Awake()
        {
            focusTarget = focus;
            pivotTarget = pivot;
            depthTarget = depth;

            ClampTargets();
            SnapToTargets();
        }

        protected virtual void Update()
        {
            ClampTargets();

            focus = Vector2.SmoothDamp(focus, focusTarget, ref focusVelocity, focusTime);
            pivot = Mathf.SmoothDamp(pivot, pivotTarget, ref pivotVelocity, pivotTime);
            depth = Mathf.SmoothDamp(depth, depthTarget, ref depthVelocity, depthTime);

            if (linkPivotToDepth)
            {
                pivotTarget = PivotFromDepth(depth);
                pivot = pivotTarget;
            }
        }

        protected virtual void ClampTargets()
        {
            pivotTarget = Mathf.Clamp(pivotTarget, minPivot, maxPivot);
            depthTarget = Mathf.Clamp(depthTarget, minDepth, maxDepth);
            focusTarget = new Vector2(Mathf.Clamp(focusTarget.x, worldBounds.xMin, worldBounds.xMax),
                                      Mathf.Clamp(focusTarget.y, worldBounds.yMin, worldBounds.yMax));
        }

        public virtual void SnapToTargets()
        {
            focus = focusTarget;
            pivot = pivotTarget;
            depth = depthTarget;
        }

        public virtual float PivotFromDepth(float depth)
        {
            float u = Mathf.InverseLerp(minDepth, maxDepth, depth);
            float v = pivotZoomCurve.Evaluate(u);
            float pivot = Mathf.Lerp(minPivot, maxPivot, v);

            return pivot;
        }
    }
}
