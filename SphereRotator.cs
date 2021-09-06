using UnityEngine;

namespace PerlinNoiseGenerator
{
    public class SphereRotator : MonoBehaviour
    {
        [Range(0, 1)] [SerializeField] private float speedOfRotation;
        
        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0, speedOfRotation, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0, -speedOfRotation, 0);
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.Rotate(0, 0, speedOfRotation);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.Rotate(0, 0, -speedOfRotation);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(-speedOfRotation, 0, 0);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(speedOfRotation, 0, 0);
            }
        }
    }
}