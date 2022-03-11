using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

namespace ATDungeon.Utility
{
    public class UtilFunctions : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerObj;

        [SerializeField]
        private Transform floorPos;

        [SerializeField]
        private GameObject cameraObj;

        [SerializeField]
        private Camera fpsCam;

        public enum Axis { x, y, z };
        public static UtilFunctions instance;
        private bool isGamePaused;

        private void Awake()
        {
            instance = this;
        }

        public Vector3 GetPlayerFacingHorizontal() { return playerObj.transform.forward; }
        public Vector3 GetPlayerFacingVertical() { return cameraObj.transform.forward; }
        public Vector3 GetPlayerPos() { return playerObj.transform.position; }
        public Vector3 GetFloorPos() { return floorPos.position; }
        public GameObject GetPlayerObj() { return playerObj; }
        public Camera GetFPSCamera() { return fpsCam; }
        public bool IsGamePaused { get => isGamePaused; set => isGamePaused = value; }

        //public bool CheckIfPrefab(GameObject go) { return PrefabUtility.IsPartOfAnyPrefab(go); }
        public bool CheckIfPrefab(GameObject go) //Work Around Solution Since Above Doesn't Work For Build
        {
            if (go.scene.name == null)
                return true;
            else return false;
        }

        //Based On https://forum.unity.com/threads/random-numbers-with-a-weighted-chance.442190/
        //Weighted Randomisation
        public int GetWeightedRandomValue(float[] weights, int arrayOffset)
        {
            if (weights == null || weights.Length == 0) return -1;

            float w;
            float t = 0;
            int i;

            for (i = 0; i < weights.Length + arrayOffset; i++)
            {
                w = weights[i];

                if (float.IsPositiveInfinity(w))
                    return i;

                else if (w >= 0f && !float.IsNaN(w))
                    t += weights[i];
            }

            float r = UnityEngine.Random.value;
            float s = 0f;

            for (i = 0; i < weights.Length + arrayOffset; i++)
            {
                w = weights[i];
                if (float.IsNaN(w) || w <= 0f) continue;

                s += w / t;
                if (s >= r) return i;
            }

            return -1;
        }

        public void DebugDrawMessage(string message, Vector3 pos)
        {
            GameObject debugMsg = new GameObject();
            debugMsg.transform.parent = this.transform;
            debugMsg.transform.position = pos;
            debugMsg.name = message;
            DrawIcon(debugMsg, 6);
        }


        public float ReturnDistance(Axis axis, Vector3 dist)
        {
            switch (axis)
            {
                case Axis.x:
                    return dist.x;
                case Axis.y:
                    return dist.y;
                case Axis.z:
                    return dist.z;
            }

            return 0;
        }

        public int ReturnRotationDifference(GameObject obj1, GameObject obj2)
        {
            float ang1 = obj1.transform.eulerAngles.y;
            float ang2 = obj2.transform.eulerAngles.y;
            float rotationDifference = Mathf.Abs(ang1 - ang2);
            if (rotationDifference > 180)
                rotationDifference = 360 - rotationDifference;

            return Mathf.RoundToInt(rotationDifference);
        }

        public int ReturnRotationDifference(float ang1, float ang2)
        {
            float rotationDifference = Mathf.Abs(ang1 - ang2);
            if (rotationDifference > 180)
                rotationDifference = 360 - rotationDifference;

            return Mathf.RoundToInt(rotationDifference);
        }

        public Vector3 ReturnDirection(Vector3 vec1, Vector3 vec2)
        {
            return (vec2 - vec1).normalized;
        }

        //Can't Build With This
        /*
        private void DrawIcon(GameObject gameObject, int idx)
        {
            var largeIcons = GetTextures("sv_label_", string.Empty, 0, 8);
            var icon = largeIcons[idx];
            var egu = typeof(EditorGUIUtility);
            var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args = new object[] { gameObject, icon.image };
            var setIcon = egu.GetMethod("SetIconForObject", flags, null, new Type[] { typeof(UnityEngine.Object), typeof(Texture2D) }, null);
            setIcon.Invoke(null, args);
        }
        */
        private void DrawIcon(GameObject gameObject, int idx) { }

        //Nor This
        /*
        private GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
        {
            GUIContent[] array = new GUIContent[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
            }
            return array;
        }
        */

        public GameObject FindClosestTag(Vector3 startPos, string tag, float distance)
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag(tag);
            GameObject closest = null;
            foreach (GameObject go in gos)
            {
                Vector3 diff = go.transform.position - startPos;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            return closest;
        }


        //From: https://answers.unity.com/questions/475066/how-to-get-a-random-point-on-navmesh.html
        public Vector3 RandomNavmeshLocation(float radius, Vector3 startPos)
        {
            Debug.Log("Attempting To Get Random NavMesh Location With Radius Of [" + radius + "] From Center: " + startPos);
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
            randomDirection = new Vector3(randomDirection.x, Mathf.Abs(randomDirection.y), randomDirection.z); // Abs of Y so position is never under the room
            randomDirection += startPos;
            Debug.Log("Random Direction: " + randomDirection);
            
            Vector3 randNavLocation = randomDirection;
            RaycastHit rayHit;
            if (Physics.Raycast(randomDirection, Vector3.down, out rayHit, 15f))
            {
                randNavLocation = rayHit.point;
                Debug.Log("Random Nav Location: " + randNavLocation);
            }
            
            NavMeshHit navHit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randNavLocation, out navHit, radius, 1))
            {
                Debug.Log("Final Position Was On NavMesh!");
                finalPosition = navHit.position;
            }
            else
            {
                Debug.LogWarning("Final Position Was Not On The NavMesh, Therefore Returning 0");
            }
            
            return finalPosition;
        }

        //From: https://answers.unity.com/questions/458207/copy-a-component-at-runtime.html
        public Component CopyComponent(Component original, GameObject destination)
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            // Copied fields can be restricted with BindingFlags
            System.Reflection.FieldInfo[] fields = type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy;
        }
    }
}

