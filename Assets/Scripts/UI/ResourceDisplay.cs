using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sjur.Resources;

namespace Sjur.UI
{
    /// <summary>
    /// Displays resource amounts in the UI
    /// </summary>
    public class ResourceDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ResourceManager resourceManager;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI ironText;
        [SerializeField] private TextMeshProUGUI arcaneText;

        [Header("Generation Rate Display")]
        [SerializeField] private TextMeshProUGUI goldRateText;
        [SerializeField] private TextMeshProUGUI ironRateText;
        [SerializeField] private TextMeshProUGUI arcaneRateText;

        [Header("Formatting")]
        [SerializeField] private bool showDecimals = false;
        [SerializeField] private bool showGenerationRate = true;

        private void Start()
        {
            if (resourceManager != null)
            {
                resourceManager.OnResourceChanged += OnResourceChanged;
                UpdateAllDisplays();
            }
        }

        private void OnDestroy()
        {
            if (resourceManager != null)
            {
                resourceManager.OnResourceChanged -= OnResourceChanged;
            }
        }

        private void Update()
        {
            // Update displays every frame for smooth resource generation display
            UpdateAllDisplays();
        }

        private void OnResourceChanged(ResourceType type, float amount)
        {
            UpdateDisplay(type);
        }

        private void UpdateAllDisplays()
        {
            if (resourceManager == null) return;

            UpdateDisplay(ResourceType.Gold);
            UpdateDisplay(ResourceType.Iron);
            UpdateDisplay(ResourceType.Arcane);
        }

        private void UpdateDisplay(ResourceType type)
        {
            if (resourceManager == null) return;

            float amount = resourceManager.GetResource(type);
            float rate = resourceManager.GetGenerationRate(type);

            string amountText = showDecimals ? amount.ToString("F1") : Mathf.Floor(amount).ToString();
            string rateText = showGenerationRate ? $"(+{rate:F1}/s)" : "";

            switch (type)
            {
                case ResourceType.Gold:
                    if (goldText != null)
                        goldText.text = $"Gold: {amountText} {rateText}";
                    break;

                case ResourceType.Iron:
                    if (ironText != null)
                        ironText.text = $"Iron: {amountText} {rateText}";
                    break;

                case ResourceType.Arcane:
                    if (arcaneText != null)
                        arcaneText.text = $"Arcane: {amountText} {rateText}";
                    break;
            }
        }

        public void SetResourceManager(ResourceManager manager)
        {
            if (resourceManager != null)
            {
                resourceManager.OnResourceChanged -= OnResourceChanged;
            }

            resourceManager = manager;

            if (resourceManager != null)
            {
                resourceManager.OnResourceChanged += OnResourceChanged;
                UpdateAllDisplays();
            }
        }
    }
}
