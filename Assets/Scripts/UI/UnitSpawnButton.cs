using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sjur.Core;
using Sjur.Themes;
using Sjur.Units;
using Sjur.Resources;

namespace Sjur.UI
{
    /// <summary>
    /// UI button for spawning units with cost display and cooldown visualization
    /// </summary>
    public class UnitSpawnButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UnitSpawner unitSpawner;
        [SerializeField] private ResourceManager resourceManager;
        [SerializeField] private ThemeManager themeManager;

        [Header("Unit Configuration")]
        [SerializeField] private UnitType unitType;

        [Header("UI Elements")]
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private TextMeshProUGUI cooldownText;

        [Header("Visual Feedback")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color cannotAffordColor = Color.red;
        [SerializeField] private Color cooldownColor = Color.gray;

        private UnitData unitData;
        private Image buttonImage;

        private void Awake()
        {
            buttonImage = button != null ? button.GetComponent<Image>() : null;

            if (button != null)
            {
                button.onClick.AddListener(OnButtonClicked);
            }
        }

        private void Start()
        {
            LoadUnitData();
            UpdateDisplay();
        }

        private void Update()
        {
            UpdateCooldownDisplay();
            UpdateAffordability();
        }

        private void LoadUnitData()
        {
            if (themeManager != null)
            {
                unitData = themeManager.GetUnitData(unitType);

                if (unitData != null && nameText != null)
                {
                    nameText.text = unitData.unitName;
                }

                UpdateCostDisplay();
            }
        }

        private void UpdateCostDisplay()
        {
            if (unitData == null || costText == null) return;

            string costString = "";
            foreach (var cost in unitData.spawnCosts)
            {
                string resourceName = cost.resourceType.ToString();
                costString += $"{resourceName}: {cost.amount}\n";
            }

            costText.text = costString.TrimEnd();
        }

        private void UpdateCooldownDisplay()
        {
            if (unitSpawner == null) return;

            float cooldown = unitSpawner.GetCooldown(unitType);
            bool isOnCooldown = cooldown > 0;

            if (cooldownOverlay != null)
            {
                cooldownOverlay.gameObject.SetActive(isOnCooldown);

                if (isOnCooldown && unitData != null)
                {
                    float fillAmount = cooldown / unitData.spawnCooldown;
                    cooldownOverlay.fillAmount = fillAmount;
                }
            }

            if (cooldownText != null)
            {
                if (isOnCooldown)
                {
                    cooldownText.gameObject.SetActive(true);
                    cooldownText.text = cooldown.ToString("F1") + "s";
                }
                else
                {
                    cooldownText.gameObject.SetActive(false);
                }
            }

            // Disable button during cooldown
            if (button != null)
            {
                button.interactable = !isOnCooldown;
            }
        }

        private void UpdateAffordability()
        {
            if (unitData == null || resourceManager == null || buttonImage == null)
                return;

            var costs = unitData.GetSpawnCostDictionary();
            bool canAfford = resourceManager.CanAffordMultiple(costs);
            bool isOnCooldown = unitSpawner != null && unitSpawner.IsOnCooldown(unitType);

            Color targetColor = normalColor;

            if (isOnCooldown)
            {
                targetColor = cooldownColor;
            }
            else if (!canAfford)
            {
                targetColor = cannotAffordColor;
            }

            buttonImage.color = targetColor;
        }

        private void OnButtonClicked()
        {
            if (unitSpawner != null)
            {
                bool success = unitSpawner.RequestSpawn(unitType);

                if (!success)
                {
                    // Play error sound or show feedback
                    Debug.Log($"Failed to spawn {unitType}");
                }
            }
        }

        private void UpdateDisplay()
        {
            LoadUnitData();
        }

        public void SetUnitSpawner(UnitSpawner spawner)
        {
            unitSpawner = spawner;
        }

        public void SetResourceManager(ResourceManager manager)
        {
            resourceManager = manager;
        }

        public void SetThemeManager(ThemeManager manager)
        {
            themeManager = manager;
            LoadUnitData();
        }

        public void SetUnitType(UnitType type)
        {
            unitType = type;
            LoadUnitData();
        }
    }
}
