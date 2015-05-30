using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PlaneCamera
{
    [RequireComponent(typeof(Camera))]
    public class CameraInput : MonoBehaviour
    {
        protected new Camera camera;

        public bool scrollZoom;
        [Range(-8, 8)]
        [Tooltip("Multiply the effect scrolling has on the camera depth target by 2^x")]
        public float scrollDepthPower;



        public bool keyboardPan;
        public bool scalePan;

        [Range(-8, 8)]
        public float panPower;

        protected virtual void Awake()
        {
            camera = GetComponent<Camera>();
        }

        protected virtual void Update()
        {
            if (scrollZoom)
            {
                camera.depthTarget -= Input.GetAxis("Mouse ScrollWheel") * Mathf.Pow(2, scrollDepthPower);
            }

            Vector2 pan = Vector2.zero;

            if (keyboardPan)
            {
                pan += Vector2.up    * Input.GetAxis("Vertical")   * Mathf.Pow(2, panPower)
                    +  Vector2.right * Input.GetAxis("Horizontal") * Mathf.Pow(2, panPower);
            }

            if (scalePan)
            {
                pan *= camera.depth / camera.maxDepth;
            }

            camera.focusTarget += pan;
        }
    }
}
