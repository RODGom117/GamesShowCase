using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{

    public enum CharacterType
    {
        Hero,
        Enemy,
        Boss
    }

    public class UIManager : MonoBehaviour
    {
        public Slider healthBar; // Reference to the health bar slider
        public Slider madnessBar; // Reference to the madness bar slider for Rosa

        // Delegate for updating health UI
        public delegate void UpdateHealthUI(float health, float maxHealth, CharacterType characterType);
        public static event UpdateHealthUI OnUpdateHealthUI;

        private void Start()
        {
            // Subscribe to the OnUpdateHealthUI event
            OnUpdateHealthUI += HandleHealthUpdated;
        }

        public void SetMaxHealth(float maxHealth)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }

        public void UpdateHealth(float health, CharacterType characterType)
        {
            // Invoke the OnUpdateHealthUI event
            OnUpdateHealthUI?.Invoke(health, healthBar.maxValue, characterType);
        }

        private void HandleHealthUpdated(float health, float maxHealth, CharacterType characterType)
        {
            // Perform different actions depending on character type
            if (characterType == CharacterType.Hero || characterType == CharacterType.Enemy)
            {
                // Update the health bar slider value
                healthBar.value = health;
            }
            else if (characterType == CharacterType.Boss)
            {
                // Handle boss-specific UI updates if needed
            }
        }

        public void UpdateMadness(float madness)
        {
            if (madnessBar != null)
            {
                madnessBar.value += madness;
            }
        }

        public float GetMadness()
        {
            return madnessBar.value;
        }

        public void SetMadness(float madness)
        {
            madnessBar.value = madness;
        }

        private void OnDestroy()
        {
            // Unsubscribe from the OnUpdateHealthUI event
            OnUpdateHealthUI -= HandleHealthUpdated;
        }
    }
}