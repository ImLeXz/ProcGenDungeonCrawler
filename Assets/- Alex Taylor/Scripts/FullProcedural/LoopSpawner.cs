using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Utility;

namespace ATDungeon.Dungeon.Setup
{
    public class LoopSpawner : MonoBehaviour
    {
        public enum HallwayType {notPossible, Lshape, Zshape };

        [SerializeField]
        private GameObject cube;
        [SerializeField]
        private Material cubeMaterial;
        [SerializeField]
        private float width = 1.0f;
        [SerializeField]
        private float thickness = 0.25f;
        [SerializeField]
        [Range(0.0f, 1.0f)] private float validatorScalePercent = 0.95f;

        GameObject startObj;
        GameObject endObj;
        float minDistance;
        float halfWidth;
        int hallwaysSpawned;

        void Start()
        {
            halfWidth = width * 0.5f;
            minDistance = halfWidth;

            cube.SetActive(false);
        }

        private void Update()
        {
        }

        public HallwayType ReturnDesiredHallwayType(GameObject from, GameObject to)
        {
            int rotDifference = UtilFunctions.instance.ReturnRotationDifference(from, to);
            Debug.Log("Rot Difference = " + rotDifference);
            switch(rotDifference)
            {
                case 90:
                    return HallwayType.Lshape;
                case 180:
                    return HallwayType.Zshape;
            }

            GameObject parentObj01 = from.GetComponent<ConnectionPoint>().GetRoomSpawner().gameObject;
            GameObject parentObj02 = to.GetComponent<ConnectionPoint>().GetRoomSpawner().gameObject;
            Debug.LogWarning("HallwayType Not Found, Rotation Difference Was: [" + rotDifference + "] Between: [" + parentObj01.name + "] and [" + parentObj02.name + "]");
            return HallwayType.notPossible;
        }

        public void SpawnHall(GameObject from, GameObject to, ConnectionPoint cp01, ConnectionPoint cp02, HallwayType hType)
        {
            startObj = from;
            endObj = to;

            //Get how many cubes are needed for the specified hallway shape
            int cubesNeeded = 0;
            switch (hType)
            {
                case HallwayType.notPossible:
                    cubesNeeded = 0;
                    return;
                case HallwayType.Lshape:
                    cubesNeeded = 2;
                    break;
                case HallwayType.Zshape:
                    cubesNeeded = 3;
                    break;
            }

            //Add all cubes to an array
            GameObject[] cubes = new GameObject[cubesNeeded];
            for (int i = 0; i < cubesNeeded; i++)
            {
                cubes[i] = Instantiate(cube);
                cubes[i].transform.position = startObj.transform.position;
            }

            bool flipped = IsFlipped(from, to, hType);
            //Debug.Log("Flipped?: " + flipped);
            //Debug.Log("Rotation Difference: " + UtilFunctions.instance.ReturnRotationDifference(from, to));
            Debug.Log("Generating Hall From: [" + from.name + " / "  + cp01.GetRoomSpawner().gameObject.name + "] to: [" + to.name + " / " + cp02.GetRoomSpawner().gameObject.name + "]");

            //Setup parent object for hallway
            GameObject hallwayParent = new GameObject();
            hallwayParent.name = (hType.ToString() + " Hallway - " + hallwaysSpawned + " [" + DungeonManager.instance.GetCurrentRoomCount() + "]");
            RoomSpawner hallwayRoom = hallwayParent.AddComponent<RoomSpawner>();

            //Setup parent object for validators
            GameObject validatorParent = new GameObject();
            validatorParent.name = "Validator Parent";
            validatorParent.transform.parent = hallwayParent.transform;
            Validator[] validators = new Validator[cubesNeeded];

            //Setup all cubes
            for (int i = 0; i < cubes.Length; i++)
            {
                SetupHallway(cubes, hType, startObj.transform.position, endObj.transform.position, flipped);
                cubes[i].transform.parent = hallwayParent.transform;
                cubes[i].SetActive(true);

                GameObject validator = Instantiate(DungeonManager.instance.GetValidatorObj());
                validator.transform.parent = validatorParent.transform;

                BoxCollider boxc = cubes[i].AddComponent<BoxCollider>();
                BoxCollider valBox = validator.GetComponent<BoxCollider>();
                valBox.size = new Vector3(boxc.size.x * validatorScalePercent, boxc.size.y * 4f, boxc.size.z * validatorScalePercent);
                validator.transform.position = cubes[i].GetComponent<Renderer>().bounds.center;
                validators[i] = validator.GetComponent<Validator>();
            }

            GameObject cpParent = new GameObject("ConnectionPoints");
            cpParent.transform.parent = hallwayRoom.gameObject.transform;

            GameObject hallCp01Obj = new GameObject("ConnectionPoint_01");
            GameObject hallCp02Obj = new GameObject("ConnectionPoint_02");
            hallCp01Obj.tag = "ConnectionPoint";
            hallCp02Obj.tag = "ConnectionPoint";

            BoxCollider boxCollider01 = hallCp01Obj.AddComponent<BoxCollider>();
            BoxCollider boxCollider02 = hallCp02Obj.AddComponent<BoxCollider>();

            Vector3 boxColSize = new Vector3(0.5f, 0.5f, 0.5f);
            boxCollider01.size = boxColSize;
            boxCollider02.size = boxColSize;
            boxCollider01.isTrigger = true;
            boxCollider02.isTrigger = true;

            hallCp01Obj.transform.position = cp01.transform.position;
            hallCp02Obj.transform.position = cp02.transform.position;
            hallCp01Obj.transform.parent = cpParent.transform;
            hallCp02Obj.transform.parent = cpParent.transform;

            ConnectionPoint hallCp01 = hallCp01Obj.AddComponent<ConnectionPoint>();
            ConnectionPoint hallCp02 = hallCp02Obj.AddComponent<ConnectionPoint>();
            hallCp01.IsConnected = true;
            hallCp02.IsConnected = true;

            ConnectionPoint[] hallCps = new ConnectionPoint[] { hallCp01, hallCp02 };

            //Setup hallway RoomSpawner component
            hallwayRoom.Validators = validators;
            hallwayRoom.ConnectionPoints = hallCps;
            hallwayRoom.RoomType = DungeonManager.RoomType.LoopingPath;
            hallwayRoom.StartValidationChecks();
            StartCoroutine(ValidateLoop(hallwayRoom, cp01, cp02, DungeonManager.instance.GetRoomValidationDelay()));
        }

        private void SetupHallway(GameObject[] cubes, HallwayType hType, Vector3 startPos, Vector3 endPos, bool flipped)
        {
            // Build the x path
            float xDir = UtilFunctions.instance.ReturnDirection(startPos, endPos).x;
            float zDir = UtilFunctions.instance.ReturnDirection(startPos, endPos).z;

            Vector3 dist = endPos - startPos;

            float zLength = Mathf.Abs(endPos.z - startPos.z);
            float xLength = Mathf.Abs(endPos.x - startPos.x);

            // X Direction Left
            if (xDir < 0)
            {
                Debug.Log("Room is on the left!");
                Vector3 scale = new Vector3(-xLength - halfWidth, thickness, width);

                //L Shape Hall
                if (hType == HallwayType.Lshape)
                {
                    if (!flipped)
                    {
                        GenerateHall(cubes[1], true, true, scale, halfWidth, -halfWidth);
                    }

                    else
                    {
                        GenerateHall(cubes[0], true, true, scale, 0.0f, -halfWidth);
                        GenerateHall(cubes[1], false, false, Vector3.zero, -(xLength) - halfWidth, 0.0f);
                    }
                }
                //###########

                //Z Shape Hall
                else if (hType == HallwayType.Zshape)
                {
                    if (!flipped)
                    {
                        Vector3 newScale = new Vector3((scale.x * 0.5f) + (width * .75f), scale.y, scale.z);
                        GenerateHall(cubes[1], true, true, newScale, 0.0f, -halfWidth);
                        GenerateHall(cubes[2], true, true, newScale, newScale.x - width, -halfWidth);
                        GenerateHall(cubes[0], false, false, Vector3.zero, (-xLength * .5f) - halfWidth, 0.0f);
                    }

                    else
                    {
                        Vector3 newScale = new Vector3(scale.x - halfWidth, scale.y, scale.z);
                        GenerateHall(cubes[0], true, true, newScale, halfWidth, 0.0f);
                        GenerateHall(cubes[2], false, false, Vector3.zero, newScale.x + halfWidth, 0.0f);
                    }
                }
                //###########
            }
			
            //X Direction Right
            else
            {
                Debug.Log("Room is on the right!");
                Vector3 scale = new Vector3(xLength + halfWidth, thickness, width);

                //L Shape Hall
                if (hType == HallwayType.Lshape)
                {
                    if (!flipped)
                    {
                        GenerateHall(cubes[1], true, false, scale, -halfWidth, -halfWidth);
                    }

                    else
                    {
                        GenerateHall(cubes[0], true, false, scale, 0.0f, -halfWidth);
                        GenerateHall(cubes[1], false, false, Vector3.zero, xLength - halfWidth, 0.0f);
                    }
                }
                //###########

                //Z Shape Hall
                else if (hType == HallwayType.Zshape)
                {
                    if (!flipped)
                    {
                        Vector3 newScale = new Vector3((scale.x * 0.5f) - (width * .75f), scale.y, scale.z);
                        GenerateHall(cubes[1], true, false, newScale, 0.0f, -halfWidth);
                        GenerateHall(cubes[2], true, false, newScale, newScale.x + width, -halfWidth);
                        GenerateHall(cubes[0], false, false, Vector3.zero, (xLength * .5f) - halfWidth, 0.0f);
                    }

                    else
                    {
                        Vector3 newScale = new Vector3(scale.x + halfWidth, scale.y, scale.z);
                        GenerateHall(cubes[0], true, false, newScale, -halfWidth, 0.0f);
                        GenerateHall(cubes[2], false, false, Vector3.zero, newScale.x - (width * 1.5f), 0.0f);
                    }
                }
                //###########
            }
            //--------------------------
			
            // Z Direction Below
            if (zDir < 0)
            {
                Debug.Log("Room is below!");
                Vector3 scale = new Vector3(width, thickness, -(zLength) + halfWidth);

                //L Shape Hall
                if (hType == HallwayType.Lshape)
                {
                    if (!flipped)
                    {
                        GenerateHall(cubes[0], true, true, scale, -halfWidth, 0.0f);
                        GenerateHall(cubes[1], false, false, Vector3.zero, 0.0f, -(zLength) - halfWidth);
                    }

                    else
                    {
                        GenerateHall(cubes[1], true, true, scale, 0.0f, -halfWidth);
                    }
                }
                //###########

                //Z Shape Hall
                else if (hType == HallwayType.Zshape)
                {
                    if (!flipped)
                    {
                        Vector3 newScale = new Vector3(scale.x, scale.y, scale.z - (width * 1.5f));
                        GenerateHall(cubes[0], true, true, newScale, 0.0f, +halfWidth);
                        GenerateHall(cubes[2], false, false, Vector3.zero, 0.0f, -(zLength) - halfWidth);
                    }

                    else
                    {
                        Vector3 newScale = new Vector3(scale.x, scale.y, (scale.z * 0.5f) + (halfWidth * .5f));
                        GenerateHall(cubes[1], true, true, newScale, -halfWidth, 0.0f);
                        GenerateHall(cubes[2], true, true, newScale, 0.0f, newScale.z - width);
                        GenerateHall(cubes[0], false, false, Vector3.zero, 0.0f, newScale.z - width);
                    }
                }
                //###########

            }
			
            // Z Direction Above
            else
            {
                Debug.Log("Room is above!");
                Vector3 scale = new Vector3(width, thickness, zLength - halfWidth);

                //L Shape Hall
                if (hType == HallwayType.Lshape)
                {
                    if (!flipped)
                    {
                        GenerateHall(cubes[0], true, false, scale, -halfWidth, 0.0f);
                        GenerateHall(cubes[1], false, false, Vector3.zero, 0.0f, (zLength - halfWidth));
                    }

                    else
                    {
                        GenerateHall(cubes[1], true, false, scale, 0.0f, halfWidth);
                    }
                }
                //###########

                //Z Shape Hall
                else if (hType == HallwayType.Zshape)
                {
                    if(!flipped)
                    {
                        Vector3 newScale = new Vector3(scale.x, scale.y, scale.z + width * 1.5f);
                        GenerateHall(cubes[0], true, false, newScale, 0.0f, -halfWidth);
                        GenerateHall(cubes[2], false, false, Vector3.zero, 0.0f, (zLength - halfWidth));
                    }

                    else
                    {
                        Vector3 newScale = new Vector3(scale.x, scale.y, (scale.z * 0.5f) - (halfWidth * .5f));
                        GenerateHall(cubes[1], true, false, newScale, -halfWidth, 0.0f);
                        GenerateHall(cubes[2], true, false, newScale, 0.0f, newScale.z + width);
                        GenerateHall(cubes[0], false, false, Vector3.zero, 0.0f, newScale.z);
                    }
                }
                //###########
            }
            //--------------------------
        }

        private void GenerateHall(GameObject cube, bool shouldGenerateCube, bool flipNormals, Vector3 scale, float xPosOffset, float zPosOffset)
        {
            if (cube != null)
            {
                Vector3 pos = cube.transform.position;

                float xOffset = startObj.transform.position.x + xPosOffset;
                float yOffset = startObj.transform.position.y - thickness;
                float zOffset = startObj.transform.position.z + zPosOffset;
                if (xPosOffset >= -0.001f && xPosOffset < 0.001f)
                    xOffset = pos.x;
                if (zPosOffset > -0.001f && zPosOffset < 0.001f)
                    zOffset = pos.z;

                cube.transform.position = new Vector3(xOffset, yOffset, zOffset);
                if (shouldGenerateCube)
                    CreateCube(cube, scale, false, flipNormals);
            }
        }


        private Vector2 AbsVector(Vector2 vec)
        {
            return new Vector2(Mathf.Abs(vec.x), Mathf.Abs(vec.y));
        }

        private void CreateCube(GameObject cube, Vector3 scale, bool flipNormals, bool flipTriangles)
        {
            Vector3[] vertices = {
            new Vector3(0, scale.y, 0),
            new Vector3(0, 0, 0),
            new Vector3(scale.x, scale.y, 0),
            new Vector3(scale.x, 0, 0),

            new Vector3(0, 0, scale.z),
            new Vector3(scale.x, 0, scale.z),
            new Vector3(0, scale.y, scale.z),
            new Vector3(scale.x, scale.y, scale.z),

            new Vector3(0, scale.y, 0),
            new Vector3(scale.x, scale.y, 0),

            new Vector3(0, scale.y, 0),
            new Vector3(0, scale.y, scale.z),

            new Vector3(scale.x, scale.y, 0),
            new Vector3(scale.x, scale.y, scale.z),
        };

            int[] triangles = {
            0, 2, 1, // front
			1, 2, 3,
            4, 5, 6, // back
			5, 7, 6,
            6, 7, 8, //top
			7, 9 ,8,
            1, 3, 4, //bottom
			3, 5, 4,
            1, 11,10,// left
			1, 4, 11,
            3, 12, 5,//right
			5, 12, 13


        };

            Vector2[] uvs = {
            new Vector2(0, 0.66f),
            new Vector2(0.25f, 0.66f),
            new Vector2(0, 0.33f),
            new Vector2(0.25f, 0.33f),

            new Vector2(0.5f, 0.66f),
            new Vector2(0.5f, 0.33f),
            new Vector2(0.75f, 0.66f),
            new Vector2(0.75f, 0.33f),

            new Vector2(1, 0.66f),
            new Vector2(1, 0.33f),

            new Vector2(0.25f, 1),
            new Vector2(0.5f, 1),

            new Vector2(0.25f, 0),
            new Vector2(0.5f, 0),
        };

            for (int i = 0; i < uvs.Length; i++)
                uvs[i] = AbsVector(uvs[i]);

            Vector3[] normals = new Vector3[14];
            Vector3 normalDirection = -Vector3.forward;

            for (int i = 0; i < normals.Length; i++)
                normals[i] = normalDirection;

            Mesh mesh = cube.GetComponent<MeshFilter>().mesh;
            cube.GetComponent<MeshRenderer>().material = cubeMaterial;

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;

            if (flipNormals)
                FlipNormals(cube);
            if (flipTriangles)
                FlipTriangles(cube);


            mesh.uv = uvs;
            mesh.Optimize();
        }

        private void FlipNormals(GameObject obj)
        {
            Debug.Log("Flipping Normals...");
            Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
            Vector3[] normals = mesh.normals;


            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;
        }

        private void FlipTriangles(GameObject obj)
        {
            Debug.Log("Flipping Triangles...");
            Mesh mesh = obj.GetComponent<MeshFilter>().mesh;

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }

        private bool IsFlipped(GameObject from, GameObject to, HallwayType hType)
        {
            int absStartX = Mathf.RoundToInt( Mathf.Abs(from.transform.right.x) );
            int absEndX = Mathf.RoundToInt( Mathf.Abs(to.transform.right.x) );
            int absStartZ = Mathf.RoundToInt( Mathf.Abs(from.transform.right.z) );
            int absEndZ = Mathf.RoundToInt( Mathf.Abs(to.transform.right.z) );

            bool flipped = false;

            switch (hType)
            {
                case HallwayType.Lshape:
                    if (Mathf.RoundToInt(absStartX) == 1 && Mathf.RoundToInt(absEndZ) == 1)
                        flipped = true;
                    break;

                case HallwayType.Zshape:
                    if (Mathf.RoundToInt(absStartX) == 0 && Mathf.RoundToInt(absEndZ) == 1)
                        flipped = true;
                    break;
            }

            return flipped;
        }

        IEnumerator ValidateLoop(RoomSpawner room, ConnectionPoint cp01, ConnectionPoint cp02, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            bool isValid = room.ValidateRoom();
            if (isValid)
            {
                hallwaysSpawned++;
                cp01.IsConnected = true;
                cp02.IsConnected = true;
                DungeonManager.instance.GetConnectionPieceFromDictionary(room.RoomType).curQuantity++; //Adds To Quantity
                DungeonManager.instance.AddRoomToList(room); //Adds Room To List Of All Rooms Spawn
                DungeonManager.instance.UpdateCurrentRoomNum(); //Sets Current Room Counter Num Plus One
            }
            else
            {
                cp01.IsConnected = false;
                cp02.IsConnected = false;
                Destroy(room.gameObject);
            }
        }

        public float GetWidth() { return width; }
        public int GetHallwaysSpawned() { return hallwaysSpawned; }

    }
}
