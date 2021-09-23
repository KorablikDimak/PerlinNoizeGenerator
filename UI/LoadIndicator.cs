using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PerlinNoiseGenerator.UI
{
    public class LoadIndicator : MonoBehaviour
    {
        [SerializeField] private Sprite checkMark;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Image loadingImage;
        [SerializeField] private Text loadingText;
        private float _progress;

        /// <summary>
        /// Refreshes the indicator every frame.
        /// Do this method only in Coroutine so as not to freeze the program while the algorithm is running.
        /// </summary>
        public IEnumerator AsyncLoadingImage()
        {
            while (true)
            {
                loadingImage.fillAmount = _progress;
                yield return null;
            }
        }
        
        public void SetCheckSprite()
        {
            loadingImage.sprite = checkMark;
        }
        
        public void SetDefaultSprite()
        {
            loadingImage.sprite = defaultSprite;
        }

        public void SetText(string text)
        {
            loadingText.text = text;
        }

        public void AddProgress(float valueToAdd)
        {
            _progress += valueToAdd;
            if (_progress > 1)
            {
                _progress = 1;
            }
        }

        public void RemoveProgress()
        {
            _progress = 0;
        }
    }
}