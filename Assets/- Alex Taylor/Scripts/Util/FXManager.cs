using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Utility
{
    public class FXManager : MonoBehaviour
    {
        public enum EffectType { Explosion, Blood, ChestDestroy, ShieldPulse, WarningCircle };
        [System.Serializable]
        public class Effect
        {
            public EffectType effectType;
            public GameObject prefab;
        }

        public static FXManager Instance;

        [SerializeField]
        private Effect[] effects;

        private Dictionary<EffectType, Effect> effectDictionary;
        private float effectDuration;

        // Start is called before the first frame update

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            effectDictionary = new Dictionary<EffectType, Effect>();
            foreach(Effect effect in effects)
            {
                effectDictionary.Add(effect.effectType, effect);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private GameObject GetEffectObj(EffectType fx)
        {
            GameObject effectObj = null;
            switch (fx)
            {
                case EffectType.Explosion:
                    effectObj = effectDictionary[EffectType.Explosion].prefab;
                    break;
                case EffectType.Blood:
                    effectObj = effectDictionary[EffectType.Blood].prefab;
                    break;
                case EffectType.ChestDestroy:
                    effectObj = effectDictionary[EffectType.ChestDestroy].prefab;
                    break;
                case EffectType.ShieldPulse:
                    effectObj = effectDictionary[EffectType.ShieldPulse].prefab;
                    break;
                case EffectType.WarningCircle:
                    effectObj = effectDictionary[EffectType.WarningCircle].prefab;
                    break;
            }
            effectDuration = effectObj.GetComponent<ParticleSystem>().main.duration;
            return effectObj;
        }

        private IEnumerator DeleteEffect(GameObject effectObj)
        {
            yield return new WaitForSeconds(effectDuration);
            Destroy(effectObj);
        }

        public void PlayEffect(EffectType fx, Vector3 pos, Material mat)
        {
            GameObject fxObj = GetEffectObj(fx);
            if (fxObj)
            {
                GameObject tempObj = Instantiate(fxObj);
                ParticleSystem ps = tempObj.GetComponent<ParticleSystem>();

                if(mat)
                    ps.GetComponent<ParticleSystemRenderer>().material = mat;

                tempObj.transform.localPosition = pos;
                tempObj.transform.SetParent(this.transform);
                tempObj.transform.localScale = new Vector3(1, 1, 1);
                StartCoroutine(DeleteEffect(tempObj));
            }
            else
            {
                Debug.LogError("Tried To Play Effect But FX Obj Is Null!");
            }
        }

        public void PlayShaderFloatEffect(string propertyName, float val, Material mat)
        {
            /*
            int propIndex = mat.shader.FindPropertyIndex(propertyName);
            UnityEngine.Rendering.ShaderPropertyType shaderProp = mat.shader.GetPropertyType(propIndex);
            */
            int propID = Shader.PropertyToID(propertyName);
            mat.SetFloat(propID, val);
        }
    }
}
