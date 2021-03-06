using System.Collections;
using PerlinNoiseGenerator.MapGen;
using UnityEngine;
using UnityEngine.UI;

namespace PerlinNoiseGenerator.RenderMap
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private NoiseMapGenerator noiseMapGenerator;
        [SerializeField] private InputField inputField;
        private Coroutine _rotateCoroutine;
        public float RotateSpeed { get; set; }
        public Transform ThisTransform { get; set; }

        private void FixedUpdate()
        {
            if (inputField.isFocused)
            {
                return;
            }
            if (Input.GetKey(KeyCode.A))
            {
                ThisTransform.Rotate(0, 1, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                ThisTransform.Rotate(0, -1, 0);
            }
            if (noiseMapGenerator.CurrentTypeOfRenderer != NoiseMapGenerator.TypeOfRenderer.Sphere) return;
            if (Input.GetKey(KeyCode.W))
            {
                ThisTransform.Rotate(0, 0, 1);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                ThisTransform.Rotate(0, 0, -1);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                ThisTransform.Rotate(-1, 0, 0);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                ThisTransform.Rotate(1, 0, 0);
            }
        }
        
        /// <summary>
        /// Starts the rotation of the game object.
        /// </summary>
        public void StartRotation()
        {
            _rotateCoroutine = StartCoroutine(RotateTransform());
        }

        private IEnumerator RotateTransform()
        {
            while (true)
            {
                ThisTransform.Rotate(0, RotateSpeed, 0);
                yield return new WaitForSeconds(1f / 30f);
            }
        }
        
        /// <summary>
        /// Stops the rotation of the game object.
        /// </summary>
        public void StopRotation()
        {
            if (_rotateCoroutine != null)
            {
                StopCoroutine(_rotateCoroutine);
            }

            ThisTransform.rotation = new Quaternion();
        }
    }
}