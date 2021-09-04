using UnityEngine;

namespace PerlinNoiseGenerator
{
    public class SphereRotator : MonoBehaviour
    {
        [SerializeField] private NoiseMapRenderer noiseMapRenderer;
        
        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                StopAllCoroutines();
                StartCoroutine(noiseMapRenderer.RotateSphere());
            }
            else if (Input.GetKey(KeyCode.D))
            {
                StopAllCoroutines();
                StartCoroutine(noiseMapRenderer.RotateSphere());
            }
            else if (Input.GetKey(KeyCode.W))
            {
                StopAllCoroutines();
                StartCoroutine(noiseMapRenderer.RotateSphere());
            }
            else if (Input.GetKey(KeyCode.S))
            {
                StopAllCoroutines();
                StartCoroutine(noiseMapRenderer.RotateSphere());
            }
        }
    }
}