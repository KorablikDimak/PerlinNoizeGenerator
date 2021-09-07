using UnityEngine;
using UnityEngine.UI;

namespace PerlinNoiseGenerator
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private NoiseMapGenerator noiseMapGenerator;
        [SerializeField] private NoiseMapRenderer noiseMapRenderer;
        [SerializeField] private Button generateButton;
        [SerializeField] private Slider sliderTemperature;
        [SerializeField] private Slider sliderWeight;
        [SerializeField] private Slider sliderGroundLevel;
        [SerializeField] private Slider sliderWaterLevel;
        [SerializeField] private InputField inputField;
        [SerializeField] private Toggle toggle;
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
            
            sliderTemperature.onValueChanged.AddListener(delegate { TemperatureChange(); });
            sliderWeight.onValueChanged.AddListener(delegate { WeightChange(); });
            sliderGroundLevel.onValueChanged.AddListener(delegate { GroundLevelChange(); });
            sliderWaterLevel.onValueChanged.AddListener(delegate { WaterLevelChange(); });
            sliderRotateSpeed.onValueChanged.AddListener(delegate { RotateSpeedChange(); });
            rotateSpeedValue.text = $"{(int) (sliderRotateSpeed.value * 30)} град/с";
        }

        private static void AppCloseButtonClicked()
        {
            Application.Quit();
        }

        private void GenerateButtonClicked()
        {
            noiseMapGenerator.mapSize = dropdownMapSize.value switch
            {
                0 => NoiseMapGenerator.MapSize.Small,
                1 => NoiseMapGenerator.MapSize.Medium,
                2 => NoiseMapGenerator.MapSize.Large,
                3 => NoiseMapGenerator.MapSize.Giant,
                _ => noiseMapGenerator.mapSize
            };

            noiseMapGenerator.typeOfRenderer = dropdownTypeOfRender.value switch
            {
                0 => NoiseMapGenerator.TypeOfRenderer.Plane,
                1 => NoiseMapGenerator.TypeOfRenderer.Sphere,
                _ => noiseMapGenerator.typeOfRenderer
            };

            noiseMapRenderer.typeOfMap = dropdownTypeOfMap.value switch
            {
                0 => NoiseMapRenderer.TypeOfMap.Simple,
                1 => NoiseMapRenderer.TypeOfMap.Weight,
                2 => NoiseMapRenderer.TypeOfMap.Height,
                3 => NoiseMapRenderer.TypeOfMap.Temperature,
                _ => noiseMapRenderer.typeOfMap
            };

            noiseMapRenderer.rivers = toggle.isOn;
            noiseMapGenerator.seed = inputField.text.GetHashCode();
            StartCoroutine(noiseMapGenerator.GenerateAll());
        }

        private void RotateSpeedChange()
        {
            noiseMapGenerator.rotateSpeed = sliderRotateSpeed.value;
            rotateSpeedValue.text = $"{(int) (sliderRotateSpeed.value * 30)} град/с";
        }

        private void TemperatureChange()
        {
            noiseMapRenderer.temperature = sliderTemperature.value;
        }
        
        private void WeightChange()
        {
            noiseMapRenderer.weight = sliderWeight.value;
        }
        
        private void GroundLevelChange()
        {
            noiseMapRenderer.groundLevel = sliderGroundLevel.value;
        }
        
        private void WaterLevelChange()
        {
            noiseMapRenderer.waterLevel = sliderWaterLevel.value;
        }
    }
}