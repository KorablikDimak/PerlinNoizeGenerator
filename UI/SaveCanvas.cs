using UnityEngine;
using UnityEngine.UI;

namespace PerlinNoiseGenerator.UI
{
    public class SaveCanvas : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Canvas saveCanvas;

        private void Start()
        {
            closeButton.onClick.AddListener(CloseButtonClicked);
        }

        private void CloseButtonClicked()
        {
            saveCanvas.enabled = false;
        }
    }
}