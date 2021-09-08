using UnityEngine;
using UnityEngine.UI;

namespace PerlinNoiseGenerator
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Rotator rotator;
        [SerializeField] private NoiseMapGenerator noiseMapGenerator;
        [SerializeField] private NoiseMapRenderer noiseMapRenderer;
        [SerializeField] private Button generateButton;
        [SerializeField] private Slider sliderTemperature;
        [SerializeField] private Slider sliderWeight;
        [SerializeField] private Slider sliderGroundLevel;
        [SerializeField] private Slider sliderWaterLevel;
        [SerializeField] private InputField inputField;
        [SerializeField] private Dropdown dropdownMapSize;
        [SerializeField] private Dropdown dropdownTypeOfMap;
        [SerializeField] private Dropdown dropdownTypeOfRender;
        [SerializeField] private Text rotateSpeedValue;
        [SerializeField] private Slider sliderRotateSpeed;
        [SerializeField] private Button appCloseButton;

        private void Start()
        {
            generateButton.onClick.AddListener(GenerateButtonClicked);
            appCloseButton.onClick.AddListener(AppCloseButtonClicked);

            inputField.characterLimit = 16;
            
            sliderTemperature.minValue = 0;
            sliderTemperature.maxValue = 2;
            sliderTemperature.value = 0.6f;
            sliderWeight.minValue = 0;
            sliderWeight.maxValue = 1;
            sliderWeight.value = 0.66f;
            sliderGroundLevel.minValue = -0.5f;
            sliderGroundLevel.maxValue = 0.5f;
            sliderGroundLevel.value = 0;
            sliderWaterLevel.minValue = 0;
            sliderWaterLevel.maxValue = 0.5f;
            sliderWaterLevel.value = 0;
            sliderRotateSpeed.minValue = 0;
            sliderRotateSpeed.maxValue = 1;
            sliderRotateSpeed.value = 0.6f;

            sliderTemperature.onValueChanged.AddListener(delegate { TemperatureChanged(); });
            sliderWeight.onValueChanged.AddListener(delegate { WeightChanged(); });
            sliderGroundLevel.onValueChanged.AddListener(delegate { GroundLevelChanged(); });
            sliderWaterLevel.onValueChanged.AddListener(delegate { WaterLevelChanged(); });
            sliderRotateSpeed.onValueChanged.AddListener(delegate { RotateSpeedChanged(); });
            rotateSpeedValue.text = $"{(int) (sliderRotateSpeed.value * 30)} град/с";
            rotator.RotateSpeed = sliderRotateSpeed.value;
        }

        private static void AppCloseButtonClicked()
        {
            Application.Quit();
        }

        private void GenerateButtonClicked()
        {
            noiseMapGenerator.CurrentMapSize = dropdownMapSize.value switch
            {
                0 => NoiseMapGenerator.MapSize.Small,
                1 => NoiseMapGenerator.MapSize.Medium,
                2 => NoiseMapGenerator.MapSize.Large,
                3 => NoiseMapGenerator.MapSize.Giant,
                _ => noiseMapGenerator.CurrentMapSize
            };

            noiseMapGenerator.CurrentTypeOfRenderer = dropdownTypeOfRender.value switch
            {
                0 => NoiseMapGenerator.TypeOfRenderer.Plane,
                1 => NoiseMapGenerator.TypeOfRenderer.Sphere,
                _ => noiseMapGenerator.CurrentTypeOfRenderer
            };

            noiseMapRenderer.CurrentTypeOfMap = dropdownTypeOfMap.value switch
            {
                0 => NoiseMapRenderer.TypeOfMap.Simple,
                1 => NoiseMapRenderer.TypeOfMap.Weight,
                2 => NoiseMapRenderer.TypeOfMap.Height,
                3 => NoiseMapRenderer.TypeOfMap.Temperature,
                _ => noiseMapRenderer.CurrentTypeOfMap
            };

            noiseMapGenerator.Seed = inputField.text.GetHashCode();
            StartCoroutine(noiseMapGenerator.GenerateAll());
        }

        private void RotateSpeedChanged()
        {
            rotator.RotateSpeed = sliderRotateSpeed.value;
            rotateSpeedValue.text = $"{(int) (sliderRotateSpeed.value * 30)} град/с";
        }

        private void TemperatureChanged()
        {
            noiseMapRenderer.Temperature = sliderTemperature.value;
        }
        
        private void WeightChanged()
        {
            noiseMapRenderer.Weight = sliderWeight.value;
        }
        
        private void GroundLevelChanged()
        {
            noiseMapRenderer.GroundLevel = sliderGroundLevel.value;
        }
        
        private void WaterLevelChanged()
        {
            noiseMapRenderer.WaterLevel = sliderWaterLevel.value;
        }
    }
}