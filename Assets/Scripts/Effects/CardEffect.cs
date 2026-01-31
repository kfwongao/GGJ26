using System.Collections.Generic;
using UnityEngine;
using MaskMYDrama.Cards;

namespace MaskMYDrama.Effects
{
    /// <summary>
    /// ScriptableObject for configuring card effects.
    /// Contains a list of effects that will be applied when the card is played.
    /// Based on CSV card descriptions from MaskMYDrama - Art Design Info.csv
    /// </summary>
    [CreateAssetMenu(fileName = "New Card Effect", menuName = "MaskMYDrama/Card Effect")]
    public class CardEffect : ScriptableObject
    {
        [Header("Effect Name")]
        [Tooltip("Name of this effect configuration")]
        public string effectName = "New Effect";
        
        [Header("Effects")]
        [Tooltip("List of effects to apply when card is played")]
        public List<CardEffectData> effects = new List<CardEffectData>();
        
        [Header("Description")]
        [Tooltip("Human-readable description of what this effect does")]
        [TextArea(3, 5)]
        public string description = "";
    }
}

