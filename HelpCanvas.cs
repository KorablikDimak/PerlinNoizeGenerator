using UnityEngine;
using UnityEngine.UI;

namespace PerlinNoiseGenerator
{
    public class HelpCanvas : MonoBehaviour
    {
        [SerializeField] private Button helpButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Canvas helpCanvas;

        private void Start()
        {
            helpCanvas.enabled = false;
            helpButton.onClick.AddListener(ShowHelp);
            closeButton.onClick.AddListener(CloseHelp);
        }

        private void ShowHelp()
        {
            helpCanvas.enabled = true;
        }

        private void CloseHelp()
        {
            helpCanvas.enabled = false;
        }
    }
}