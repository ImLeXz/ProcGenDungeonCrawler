using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Utility;

namespace ATDungeon.Dungeon.Testing
{
    public class HallGenerationOptimisedBackup : MonoBehaviour
    {

        [SerializeField]
        private GameObject startPosObj;
        [SerializeField]
        private GameObject endPosObj;
        [SerializeField]
        private float width = 1.0f;
        [SerializeField]
        private float thickness = 0.25f;
        [SerializeField]
        private float maxLength = 20.0f;
        [SerializeField]
        private Material cubeMaterial;
        [SerializeField]
        private GameObject cube_01;
        [SerializeField]
        private GameObject cube_02;

        Vector3 distanceBetweenPoints = Vector3.zero;
        Vector3 lastDistance = Vector3.zero;

        Vector3 cubeScale = Vector3.zero;
        Vector3 lastDist = Vector3.zero;
        float minDistance;
        float halfWidth;
        bool flipped;


        // Start is called before the first frame update
        void Start()
        {
            halfWidth = width * 0.5f;
            minDistance = halfWidth;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 startPos = startPosObj.transform.position;
            Vector3 endPos = endPosObj.transform.position;
            Vector3 dist = endPos - startPos;
            float distanceX = Mathf.Abs(UtilFunctions.instance.ReturnDistance(UtilFunctions.Axis.x, dist));
            float distanceZ = Mathf.Abs(UtilFunctions.instance.ReturnDistance(UtilFunctions.Axis.z, dist));

            if ((distanceX > minDistance) && (distanceZ > minDistance))
            {
                if (lastDist != dist)
                {
                    SetupHallway(startPos, endPos);
                    SetupHallway(startPos, endPos);
                    Debug.Log("Flipped? = " + IsFlipped());
                    lastDist = dist;
                }
            }
        }

        private void SetupHallway(Vector3 startPos, Vector3 endPos)
        {
            // Build the x path
            float xDir = UtilFunctions.instance.ReturnDirection(startPos, endPos).x;
            float zDir = UtilFunctions.instance.ReturnDirection(startPos, endPos).z;

            Vector3 dist = endPos - startPos;

            float zLength = Mathf.Abs(endPos.z - startPos.z);
            float xLength = Mathf.Abs(endPos.x - startPos.x);

            if (xDir < 0)
            {
                Debug.Log("Room is on the left!");

                Vector3 scale = new Vector3(-xLength - halfWidth, thickness, width);
                Vector3 pos02 = cube_02.transform.position;

                cube_02.transform.position = new Vector3(this.transform.position.x + halfWidth, pos02.y, this.transform.position.z - halfWidth);
                CreateCube(cube_02, scale, false, true);
            }

            else
            {
                Debug.Log("Room is on the right!");

                Vector3 scale = new Vector3(xLength + halfWidth, thickness, width);
                Vector3 pos02= cube_02.transform.position;

                cube_02.transform.position = new Vector3(this.transform.position.x - halfWidth, pos02.y, this.transform.position.z - halfWidth);
                CreateCube(cube_02, scale, false, false);
            }

            if (zDir < 0)
            {
                Debug.Log("Room is below!");

                Vector3 scale = new Vector3(width, thickness, -(zLength) + halfWidth);
                Vector3 pos01 = cube_01.transform.position;

                cube_01.transform.position = new Vector3(this.transform.position.x - halfWidth, pos01.y, pos01.z);
                CreateCube(cube_01, scale, false, true);

                Vector3 pos02 = cube_02.transform.position;
                cube_02.transform.position = new Vector3(pos02.x, pos02.y, this.transform.position.z - (zLength + halfWidth));
            }

            else
            {
                Debug.Log("Room is above!");

                Vector3 scale = new Vector3(width, thickness, zLength - halfWidth);
                Vector3 pos01 = cube_01.transform.position;

                cube_01.transform.position = new Vector3(this.transform.position.x - halfWidth, pos01.y, pos01.z);
                CreateCube(cube_01, scale, false, false);

                Vector3 pos02 = cube_02.transform.position;
                cube_02.transform.position = new Vector3(pos02.x, pos02.y, this.transform.position.z + (zLength - halfWidth));
            }

        }

        private void GenerateHall(GameObject cube, bool shouldGenerateCube, bool flipNormals, float xLength, float zLength, float xScaleOffset, float xPosOffset, float zScaleOffset, float zPosOffset)
        {
            Vector3 scale = new Vector3(xScaleOffset, thickness, zScaleOffset);
            Vector3 pos = cube.transform.position;
            cube.transform.position = new Vector3(xPosOffset, pos.y, zPosOffset);
            if (shouldGenerateCube)
                CreateCube(cube, scale, false, flipNormals);
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

        private bool IsFlipped()
        {
            float absStartX = Mathf.Abs(startPosObj.transform.right.x);
            float absEndX = Mathf.Abs(endPosObj.transform.right.x);
            float absStartZ = Mathf.Abs(startPosObj.transform.right.z);
            float absEndZ = Mathf.Abs(endPosObj.transform.right.z);

            if (Mathf.RoundToInt(absStartX) == 1 && Mathf.RoundToInt(absEndZ) == 1)
                return true;
            else
                return false;
        }

    }
}
