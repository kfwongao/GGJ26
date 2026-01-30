/// <summary>
/// Core type definitions for the card system.
/// Based on CSV requirements: Card types (Attack, Defence, Strength, Function) and rarities (Basic, Junior, Senior).
/// </summary>
namespace MaskMYDrama.Core
{
    /// <summary>
    /// Defines the four card types as specified in the CSV gameplay design.
    /// - Attack: Deals damage to enemies
    /// - Defence: Provides block/defence points
    /// - Strength: Increases attack power
    /// - Function: Special utility effects
    /// </summary>
    public enum CardType
    {
        Attack,
        Defence,
        Strength,
        Function
    }

    /// <summary>
    /// Card rarity levels that correspond to energy costs.
    /// Based on CSV: Basic (0 energy), Junior (1 energy), Senior (2-3 energy).
    /// </summary>
    public enum CardRarity
    {
        Basic,   // 0 energy
        Junior,  // 1 energy
        Senior   // 2-3 energy
    }
}

