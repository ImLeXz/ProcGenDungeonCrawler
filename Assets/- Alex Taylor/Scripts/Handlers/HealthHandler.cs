using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Utility;

namespace ATDungeon.Handlers
{
    public class HealthHandler : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField]
        private float maxHealth = 100.0f;
        [SerializeField]
        private float healthRegenMul = 1.0f;
        [SerializeField]
        private bool bAutoRegenHealth = false;

        [Header("Shield Settings")]
        [SerializeField]
        private float maxShield = 20.0f;
        [SerializeField]
        private float shieldRegenMul = 1.0f;
        [SerializeField]
        private bool bAutoRegenShield = true;

        private float currentHealth, currentShield;

        public void Initialise()
        {
            currentHealth = maxHealth;
            currentShield = maxShield;
        }

        // Update is called once per frame
        void Update()
        {
            RegenShield();
            RegenHealth();
        }

        private void RegenShield()
        {
            if (bAutoRegenShield && currentShield < maxShield && currentHealth > 0)
            {
                currentShield += Time.deltaTime * shieldRegenMul;
            }
        }

        private void RegenHealth()
        {
            if (bAutoRegenHealth && currentHealth < maxHealth)
            {
                currentHealth += Time.deltaTime * healthRegenMul;
            }
        }

        public void ChangeHealthBy(float val, bool ignoreShield)
        {
            float resultHealth;
            float resultShield;
            if (currentShield <= 0 || ignoreShield)
            {
                resultHealth = currentHealth + val;
                if (resultHealth > 0)
                {
                    if (resultHealth < maxHealth)
                        currentHealth = resultHealth;
                    else
                    {
                        currentHealth = maxHealth;
                        resultShield = currentShield + (resultHealth - maxHealth);
                        if (resultShield > maxShield)
                            currentShield = maxShield;
                        else
                            currentShield = resultShield;
                    }
                }
                else
                    currentHealth = 0.0f;
            }

            else
            {
                resultShield = currentShield + val;
                if (resultShield > 0)
                {
                    if (resultShield > maxShield)
                        currentShield = maxShield;
                    else
                        currentShield = resultShield;
                }
                else
                {
                    resultHealth = currentHealth + (val + currentShield);
                    currentShield = 0;
                    if (resultHealth > 0)
                        currentHealth = resultHealth;
                    else
                        currentHealth = 0;
                }
            }

        }

        public void ChangeShieldRegenMultiplierBy(float val) { shieldRegenMul += val; }
        public void ChangeHealthRegenMultiplierBy(float val) { healthRegenMul += val; }
        public void ChangeMaxShieldBy(float val) { maxShield += val; }
        public void ChangeMaxHealthBy(float val) { maxHealth += val; }

        public bool GetCanAutoRegenHealth() { return bAutoRegenHealth; }
        public bool GetCanAutoRegenShield() { return bAutoRegenShield; }
        public void SetCanAutoRegenHealth(bool b) { bAutoRegenHealth = b; }
        public void SetCanAutoRegenShield(bool b) { bAutoRegenShield = b; }

        public float GetHealthRegenMultiplier() { return healthRegenMul; }
        public float GetShieldRegenMultiplier() { return shieldRegenMul; }

        public float GetMaxHealth() { return maxHealth; }
        public float GetMaxShield() { return maxShield; }

        public float GetHealth() { return currentHealth; }
        public float GetShield() { return currentShield; }
    }
}
