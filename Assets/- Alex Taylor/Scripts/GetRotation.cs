using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Dungeon.Testing
{
    public class GetRotation : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Debug.LogWarning("Rotation: " + this.transform.eulerAngles.y);
            Debug.LogWarning("Local Rotation: " + this.transform.localEulerAngles.y);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
