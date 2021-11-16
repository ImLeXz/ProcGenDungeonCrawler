using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectRooms : MonoBehaviour
{
    [SerializeField]
    private Transform room1, room2;

    [SerializeField]
    private GameObject connectingRooms;

    void Update()
    {
        Debug.Log(ReturnDirection());
        if (Input.GetMouseButtonDown(0))
        {
            BuildPath();
        }
    }

    float ReturnDistance(string axis)
    {
        Vector3 dist = room2.position - room1.position;

        if (axis == "x") return dist.x;
        if (axis == "z") return dist.z;

        return 0;
    }

    Vector3 ReturnDirection()
    {
        return (room2.position - room1.position).normalized;
    }

    void BuildPath()
    {
        // Build the x path
        float xDir = ReturnDirection().x;
        float zDir = ReturnDirection().z;

        float xDist = Mathf.Abs(ReturnDistance("x")); // this is how many cubes to spawn!
        float zDist = Mathf.Abs(ReturnDistance("z")); // this is how many cubes to spawn!

        if(xDir < 0)
        {
            SpawnConnectors("x", -1, xDist - 1, room1.position); // The room is on the right
        }
        else
        {
            SpawnConnectors("x", 1, xDist - 1, room1.position);
        }
        
        if(zDir > 0)
        {
            SpawnConnectors("z", -1, zDist, room2.position);
        }
        else
        {
            SpawnConnectors("z", 1, zDist, room2.position);
        }

    }

    void SpawnConnectors(string axis, int dir, float amount, Vector3 startingPoint)
    {
        for (int i = 0; i < amount; i++)
        {
            switch(axis)
            {
                case "x":
                    startingPoint.x += dir;
                    break;
                case "z":
                    startingPoint.z += dir;
                    break;
            }
            
            Instantiate(connectingRooms, startingPoint, connectingRooms.transform.rotation);
        }
    }
}
