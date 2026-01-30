using UnityEngine;
using TMPro;

namespace MaskMYDrama.UI
{
    /// <summary>
    /// Energy display component showing current/max energy.
    /// 
    /// Displays energy in format: "current/max" (e.g., "3/3", "2/4").
    /// Updates when player consumes energy or starts new turn.
    /// 
    /// Based on CSV: "Energy balls: Player" - displays energy available per turn.
    /// </summary>
    public class EnergyDisplay : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI energyText;
        
        public void UpdateEnergy(int current, int max)
        {
            if (energyText != null)
            {
                energyText.text = $"{current}/{max}";
            }
        }
    }
}

