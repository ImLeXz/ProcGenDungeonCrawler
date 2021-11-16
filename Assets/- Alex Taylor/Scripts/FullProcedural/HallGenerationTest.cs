using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ATDungeon.Utility;

namespace ATDungeon.Dungeon.Testing
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class HallGenerationTest : MonoBehaviour
    {
        [SerializeField]
        private GameObject startPosObj;
        [SerializeField]
        private GameObject endPosObj;
        [SerializeField]
        private float minDistance = 1.0f;
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
        float positionOffsetZ = 0.0f;
        float positionOffsetX = 0.0f;
        float scaleOffset = 0.0f;
        Vector3 lastDist = Vector3.zero;
        bool flipped;

        void Start()
        {
            positionOffsetX = width / 2;
        }

        private void Update()
        {
            Vector3 startPos = startPosObj.transform.position;
            Vector3 endPos = endPosObj.transform.position;
            Vector3 dist = endPos - startPos;
            float distanceX = Mathf.Abs(UtilFunctions.instance.ReturnDistance(UtilFunctions.Axis.x, dist));
            float distanceZ = Mathf.Abs(UtilFunctions.instance.ReturnDistance(UtilFunctions.Axis.z, dist));

            float absStartX = Mathf.Abs(startPosObj.transform.right.x);
            float absEndX = Mathf.Abs(endPosObj.transform.right.x);
            float absStartZ = Mathf.Abs(startPosObj.transform.right.z);
            float absEndZ = Mathf.Abs(endPosObj.transform.right.z);

            if (Mathf.RoundToInt(absStartX) == 1 && Mathf.RoundToInt(absEndZ) == 1)
                flipped = true;
            else if (Mathf.RoundToInt(absStartZ) == 1 && Mathf.RoundToInt(absEndX) == 1)
                flipped = false;

            Debug.Log("StartObj Right = " + startPosObj.transform.right);
            Debug.Log("EndObj Right = " + endPosObj.transform.right);
            Debug.Log("Flipped? = " + flipped);
            Debug.Log("Angle = " + Vector3.Angle(startPosObj.transform.right, endPosObj.transform.right));

            if ((distanceX > minDistance) && (distanceZ > minDistance))
            {
                if (lastDist != dist)
                {
                    GenHallTest2(startPos, endPos);
                    GenHallTest2(startPos, endPos);
                    lastDist = dist;

                    /*
                    if (flipped)
                    {
                        Vector3 cube01pos = cube_01.transform.position;
                        Vector3 cube02pos = cube_02.transform.position;
                        Debug.Log("Flipping Direction...");
                        Debug.Log("Cube 01 Initial Pos: " + cube01pos);
                        Debug.Log("Cube 02 Initial Pos: " + cube02pos);
                        cube_02.transform.position = new Vector3(-cube01pos.z, this.transform.position.y, cube01pos.x);
                        cube_01.transform.position = new Vector3(-cube02pos.z, this.transform.position.y, cube02pos.x);
                    }
                    */
                }
            }
        }

        private void GenHallTest2(Vector3 startPos, Vector3 endPos)
        {
            float startRot = startPosObj.transform.eulerAngles.y;
            float endRot = endPosObj.transform.eulerAngles.y;

            if (startRot > 180)
                startRot = 360.0f - startRot;
            if (endRot > 180)
                endRot = 360.0f - endRot;

            float rotationDiff = startRot - endRot;
            Debug.Log(startRot + " - " + endRot + " = " + rotationDiff);
            if (rotationDiff == 90)
                Debug.Log("Rotations Matched");

            // Build the x path
            float xDir = UtilFunctions.instance.ReturnDirection(startPos, endPos).x;
            float zDir = UtilFunctions.instance.ReturnDirection(startPos, endPos).z;

            Vector3 dist = endPos - startPos;

            float zLength = Mathf.Abs(endPos.z - startPos.z);
            float xLength = Mathf.Abs(endPos.x - startPos.x);

            Debug.Log("xLength: " + xLength);
            Debug.Log("zLength: " + zLength);

            if (xDir < 0)
            {
                Debug.Log("Room is on the left!");
                Vector3 scale = new Vector3(-xLength - positionOffsetX, thickness, width);
                Vector3 pos = cube_02.transform.position;

                float tempOffsetX = this.transform.position.x + positionOffsetX; ;
                float tempOffsetZ = this.transform.position.z + (positionOffsetZ - positionOffsetX);

                cube_02.transform.position = new Vector3(tempOffsetX, pos.y, tempOffsetZ);

                CreateCube(cube_02, scale, false, true);
            }

            else
            {
                Debug.Log("Room is on the right!");
                Vector3 scale = new Vector3(xLength + positionOffsetX, thickness, width);
                Vector3 pos = cube_02.transform.position;

                float tempOffsetX = this.transform.position.x - positionOffsetX;
                float tempOffsetZ = this.transform.position.z + (positionOffsetZ - positionOffsetX);

                cube_02.transform.position = new Vector3(tempOffsetX, pos.y, tempOffsetZ);

                CreateCube(cube_02, scale, false, false);
            }

            if (zDir < 0)
            {
                Debug.Log("Room is below!");
                scaleOffset = width;
                Vector3 pos01 = cube_01.transform.position;

                Vector3 scale = Vector3.zero;

                scale = new Vector3(width, thickness, -(zLength) + scaleOffset - positionOffsetX);
                positionOffsetZ = -zLength;

                float tempOffsetX = this.transform.position.x - positionOffsetX;
                float tempOffsetZ = pos01.z;

                CreateCube(cube_01, scale, false, true);
                cube_01.transform.position = new Vector3(tempOffsetX, pos01.y, tempOffsetZ);
            }

            else
            {
                Debug.Log("Room is above!");
                positionOffsetZ = zLength;
                scaleOffset = 0.0f;

                Vector3 scale = new Vector3(width, thickness, zLength - positionOffsetX);
                CreateCube(cube_01, scale, false, false);
                Vector3 pos01 = cube_01.transform.position;

                float tempOffsetX = this.transform.position.x - positionOffsetX;
                float tempOffsetZ = pos01.z;

                cube_01.transform.position = new Vector3(tempOffsetX, pos01.y, tempOffsetZ);
            }

            Debug.Log("Pos Offset Z = " + positionOffsetZ);
            Debug.Log("Pos Offset X = " + positionOffsetX);
            Debug.Log("Scale Offset = " + scaleOffset);
        }

        private void GenHallTest1()
        {
            Vector3 startPos = startPosObj.transform.position;
            Vector3 endPos = endPosObj.transform.position;

            Vector3 startRot = startPosObj.transform.right;
            Vector3 endRot = endPosObj.transform.right;

            distanceBetweenPoints = endPos - startPos;

            if (distanceBetweenPoints != lastDistance)
            {
                Debug.Log("Distance Between Points: " + distanceBetweenPoints.magnitude);
                this.transform.position = startPos;

                GetAngle(startPos, endPos);

                Vector3 vecN = Vector3.Cross(startPos, endPos);
                float angleSigned = AngleSigned(startPos, endPos, vecN);
                Debug.Log("Ang: " + angleSigned);
                float angRadians = angleSigned * (3.1415927f / 180.0f);
                Debug.Log("Ang (Radians): " + angRadians);
                float distanceAC = Mathf.Cos(angRadians) * distanceBetweenPoints.magnitude;
                Debug.Log("Distance AC = " + distanceAC);

                float distanceCB = Mathf.Sqrt((distanceBetweenPoints.magnitude * distanceBetweenPoints.magnitude) - (distanceAC * distanceAC));
                Debug.Log("Distance CB = " + distanceCB);

                Debug.Log((distanceAC * distanceAC) + " + " + (distanceCB * distanceCB) + " = " + ((distanceAC * distanceAC) + (distanceCB * distanceCB)) + " [ " + (distanceBetweenPoints.magnitude * distanceBetweenPoints.magnitude) + " ]");
                lastDistance = distanceBetweenPoints;

                //Distance Fully Correct

                Vector3 cube1scale = new Vector3(distanceAC, thickness, distanceCB);
                Debug.Log("Transform * distance: " + cube1scale);
                CreateCube(cube_01, cube1scale, false, false);

                /*
                cubeScale = new Vector3(distanceCB, thickness, width);
                GameObject cube2 = CreateCube(cubeScale);
                cube2.transform.Rotate(cube2.transform.up, -(Vector3.Angle(startRot, endRot) / 2));
                cube2.transform.position = new Vector3(cube2.transform.position.x + distanceAC, cube2.transform.position.y, cube2.transform.position.z);
                */
            }
        }

        private float GetAngle(Vector3 start, Vector3 end)
        {
            Vector2 startPos = new Vector2(Mathf.Abs(start.x), Mathf.Abs(start.z));
            Vector2 endPos = new Vector2(Mathf.Abs(end.x), Mathf.Abs(end.z));
            Debug.Log("Start Pos: " + startPos);
            Debug.Log("End Pos: " + endPos);
            float dotp = Vector2.Dot(startPos, endPos);
            float startMag = startPos.magnitude;
            float endMag = endPos.magnitude;
            float cosA = dotp / (startMag * endMag);
            float a = Mathf.Acos(cosA);
            Debug.Log("Dot Product: " + dotp);
            Debug.Log("Start Magnitude: " + startMag);
            Debug.Log("End Magnitude: " + endMag);
            Debug.Log("Cos A: " + cosA);
            Debug.Log("A: " + a);
            return 0.0f;
        }

        //Function For Calculating Angle Between Two Vectors From: https://forum.unity.com/threads/need-vector3-angle-to-return-a-negtive-or-relative-value.51092/#post-324018
        private float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
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

            /*
            Vector2[] uvs = {
            new Vector2(0, 0.66f * scale.z),
            new Vector2(0.25f * scale.x, 0.66f * scale.z),
            new Vector2(0, 0.33f * scale.z),
            new Vector2(0.25f * scale.x, 0.33f *scale.z),

            new Vector2(0.5f * scale.x, 0.66f * scale.z),
            new Vector2(0.5f * scale.x, 0.33f * scale.z),
            new Vector2(0.75f * scale.x, 0.66f * scale.z),
            new Vector2(0.75f * scale.x, 0.33f * scale.z),

            new Vector2(1 * scale.x, 0.66f * scale.z),
            new Vector2(1 * scale.x, 0.33f * scale.z),

            new Vector2(0.25f * scale.x, 1 * scale.z),
            new Vector2(0.5f * scale.x, 1 * scale.z),

            new Vector2(0.25f * scale.x, 0),
            new Vector2(0.5f * scale.x, 0),
        };
        */
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

            /*
            GameObject cube = new GameObject("Hall_Cube");
            cube.transform.parent = this.transform;
            cube.transform.localPosition = Vector3.zero;
            Mesh mesh = cube.AddComponent<MeshFilter>().mesh;
            cube.AddComponent<MeshRenderer>().material = cubeMaterial;
            */

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
    }
}
