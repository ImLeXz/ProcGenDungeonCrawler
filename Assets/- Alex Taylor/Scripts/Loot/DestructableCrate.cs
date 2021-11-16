using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Handlers;
using ATDungeon.Utility;

namespace ATDungeon.Dungeon.Loot
{
    public class DestructableCrate : MonoBehaviour
    {
        [Header("Crate Settings")]
        [SerializeField]
        private GameObject objectToDrop;
        [SerializeField]
        [Range(0.0f,1.0f)]private float chanceToDrop;

        [Header("References")]
        [SerializeField]
        private HitBoxHandler hitBox;

        [Header("FX Settings")]
        [SerializeField]
        private MeshRenderer mesh;
        [SerializeField]
        private int matIndex;
        [SerializeField]
        private string damageEffectName = "";

        HealthHandler healthHandler;
        Material matToEffect;

        private void Awake()
        {
            healthHandler = this.GetComponent<HealthHandler>();
        }

        // Start is called before the first frame update
        void Start()
        {
            hitBox.SetHitBoxCallBack(ApplyDamage);
            if (mesh is null)
                mesh = this.gameObject.GetComponentInChildren<MeshRenderer>();
            if(healthHandler)
                healthHandler.Initialise();
            matToEffect = Instantiate(mesh.materials[matIndex]);
            mesh.materials[matIndex] = matToEffect;
            mesh.sharedMaterials[matIndex] = matToEffect;
        }

        private void ApplyDamage(float dmg, bool ignoreShield)
        {
            healthHandler.ChangeHealthBy(-dmg, ignoreShield);
            if (healthHandler.GetHealth() <= 0)
                DestroyCrate();
            else
            {
                float val = 1.0f - (healthHandler.GetHealth() / healthHandler.GetMaxHealth());
                FXManager.Instance.PlayShaderFloatEffect(damageEffectName, val, matToEffect);
            }
        }

        private void DestroyCrate()
        {
            //Crate Death Animation

            float[] weights = new float[] { chanceToDrop, 1.0f - chanceToDrop };
            int shouldSpawn = UtilFunctions.instance.GetWeightedRandomValue(weights, 0);
            if (shouldSpawn == 0)
            {
                GameObject spawnedObj = Instantiate(objectToDrop, this.gameObject.transform);
                spawnedObj.transform.parent = DungeonManager.instance.GetItemSpawnParent();
            }
            Destroy(this.gameObject);
        }
        
    }
}
