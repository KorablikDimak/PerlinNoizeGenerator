using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PerlinNoiseGenerator
{
    public class Rotator : MonoBehaviour
    {
        [HideInInspector] public Transform thisTransform;
        [SerializeField] private NoiseMapGenerator noiseMapGenerator;
        [SerializeField] private InputField inputField;
        [Range(0, 0.1f)] public float rotateSpeed;

        private void FixedUpdate()
        {
            if (inputField.isFocused)
            {
                return;
            }
            if (Input.GetKey(KeyCode.A))
            {
                thisTransform.Rotate(0, 1, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                thisTransform.Rotate(0, -1, 0);
            }
            if (noiseMapGenerator.typeOfRenderer != NoiseMapGenerator.TypeOfRenderer.Sphere) return;
            if (Input.GetKey(KeyCode.W))
            {
                thisTransform.Rotate(0, 0, 1);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                thisTransform.Rotate(0, 0, -1);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                thisTransform.Rotate(-1, 0, 0);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                thisTransform.Rotate(1, 0, 0);
            }
        }
        
        public IEnumerator RotateTransform()
        {
            while (true)
            {
                thisTransform.Rotate(0, rotateSpeed, 0);
                yield return new WaitForSeconds(1f / 30f);
            }
        }
    }
}