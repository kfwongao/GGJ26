using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MaskMYDrama.UI
{
    /// <summary>
    /// Health bar display component for player and enemy.
    /// 
    /// Displays health as both a slider and text (current/max format).
    /// Used for both player and enemy health bars.
    /// 
    /// Based on CSV: "life bars: Player, Enemy"
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [Header("UI References")]
        public Slider healthSlider;
        public TextMeshProUGUI maxhealthText;
        public TextMeshProUGUI currenthealthText;

        private int currentHealth;
        private int maxHealth;
        
        public void Initialize(int maxHP)
        {
            maxHealth = maxHP;
            currentHealth = maxHP;
            UpdateDisplay();
        }
        
        public void SetHealth(int health)
        {
            currentHealth = Mathf.Clamp(health, 0, maxHealth);
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
            
            if (maxhealthText != null)
            {
                maxhealthText.text = $"{maxHealth}";
            }

            if (currenthealthText != null)
            {
                currenthealthText.text = $"{currentHealth}";
            }
        }
    }
}

