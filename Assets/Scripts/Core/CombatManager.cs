using System.Collections;
using UnityEngine;
using MaskMYDrama.Combat;
using MaskMYDrama.Cards;
using MaskMYDrama.Core;

namespace MaskMYDrama.Core
{
    /// <summary>
    /// Combat state machine states.
    /// Controls the flow of combat turns and outcomes.
    /// </summary>
    public enum CombatState
    {
        PlayerTurn,  // Player can play cards
        EnemyTurn,   // Enemy executes attack
        Victory,     // All enemies defeated
        Defeat       // Player health reached 0
    }
    
    /// <summary>
    /// Manages turn-based combat flow as specified in CSV gameplay design.
    /// 
    /// Implements the combat system:
    /// - Turn-based: Player first, then enemy (as per CSV: "Player and enemy move in turn in each round; Player first, then enemy")
    /// - Energy system: Player can only play cards if energy >= card cost
    /// - Win condition: All enemies' life bar go to 0
    /// - Lose condition: Player's life bar go to 0
    /// - Auto-end turn: When player has no enough energy for any card in hand
    /// 
    /// Based on CSV requirements:
    /// - "Player's move: Can, energy balls > the chosen card"
    /// - "Enemy's Turn: Player has no enough energy for any card in 手牌"
    /// - "Player's win: In each round, all enemies life bar go to 0"
    /// - "Player's lose: In each round, player's life bar go to 0"
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        [Header("Combat Entities")]
        public Player player;
        public Enemy enemy;
        
        [Header("Managers")]
        public DeckManager deckManager;
        
        private CombatState currentState = CombatState.PlayerTurn;
        
        public CombatState CurrentState => currentState;
        
        // Events
        public System.Action<CombatState> OnStateChanged;
        public System.Action OnPlayerTurnStart;
        public System.Action OnEnemyTurnStart;
        public System.Action OnCombatEnd;
        
        private void Start()
        {
            StartCombat();
        }
        
        public void StartCombat()
        {
            currentState = CombatState.PlayerTurn;
            StartPlayerTurn();
        }
        
        public void StartPlayerTurn()
        {
            currentState = CombatState.PlayerTurn;
            player.ResetTurn();
            deckManager.DrawHand();
            OnPlayerTurnStart?.Invoke();
            OnStateChanged?.Invoke(currentState);
        }
        
        public bool TryPlayCard(int handIndex)
        {
            if (currentState != CombatState.PlayerTurn)
                return false;
            
            if (handIndex < 0 || handIndex >= deckManager.HandCount)
                return false;
            
            var cardInstance = deckManager.GetHand()[handIndex];
            
            // Check energy
            if (!player.HasEnoughEnergy(cardInstance.GetEnergyCost()))
                return false;
            
            // Play the card
            CardInstance playedCard = deckManager.PlayCard(handIndex);
            if (playedCard == null)
                return false;
            
            // Consume energy
            player.ConsumeEnergy(playedCard.GetEnergyCost());
            
            // Apply card effects
            ApplyCardEffects(playedCard);
            
            // Check if player can still play cards
            if (!CanPlayerPlayAnyCard())
            {
                // Auto end turn if no cards can be played
                EndPlayerTurn();
            }
            
            return true;
        }
        
        private void ApplyCardEffects(CardInstance card)
        {
            switch (card.cardData.cardType)
            {
                case CardType.Attack:
                    int attackDamage = card.GetAttackValue() + player.attackPower;
                    enemy.TakeDamage(attackDamage);
                    break;
                    
                case CardType.Defence:
                    player.AddDefence(card.GetDefenceValue());
                    break;
                    
                case CardType.Strength:
                    player.AddAttackPower(card.cardData.strengthValue);
                    break;
                    
                case CardType.Function:
                    // Special function cards
                    if (card.cardData.drawCard)
                    {
                        // Draw additional card (simplified - would need proper implementation)
                    }
                    break;
            }
            
            // Check win condition
            if (!enemy.IsAlive())
            {
                currentState = CombatState.Victory;
                OnStateChanged?.Invoke(currentState);
                OnCombatEnd?.Invoke();
            }
        }
        
        private bool CanPlayerPlayAnyCard()
        {
            foreach (var card in deckManager.GetHand())
            {
                if (player.HasEnoughEnergy(card.GetEnergyCost()))
                    return true;
            }
            return false;
        }
        
        public void EndPlayerTurn()
        {
            if (currentState != CombatState.PlayerTurn)
                return;
            
            // Move unused cards to abandoned
            deckManager.EndRound();
            
            // Check lose condition
            if (!player.IsAlive())
            {
                currentState = CombatState.Defeat;
                OnStateChanged?.Invoke(currentState);
                OnCombatEnd?.Invoke();
                return;
            }
            
            // Start enemy turn
            StartCoroutine(EnemyTurnCoroutine());
        }
        
        private IEnumerator EnemyTurnCoroutine()
        {
            currentState = CombatState.EnemyTurn;
            OnEnemyTurnStart?.Invoke();
            OnStateChanged?.Invoke(currentState);
            
            // Wait a moment for visual feedback
            yield return new WaitForSeconds(0.5f);
            
            // Enemy attacks
            enemy.ExecuteAttack(player);
            
            // Wait for attack animation
            yield return new WaitForSeconds(1f);
            
            // Check lose condition
            if (!player.IsAlive())
            {
                currentState = CombatState.Defeat;
                OnStateChanged?.Invoke(currentState);
                OnCombatEnd?.Invoke();
                yield break;
            }
            
            // Reset enemy for next turn
            enemy.ResetTurn();
            
            // Start next player turn
            yield return new WaitForSeconds(0.5f);
            StartPlayerTurn();
        }
        
        public bool IsPlayerTurn()
        {
            return currentState == CombatState.PlayerTurn;
        }
        
        public bool IsCombatActive()
        {
            return currentState == CombatState.PlayerTurn || currentState == CombatState.EnemyTurn;
        }
    }
}

