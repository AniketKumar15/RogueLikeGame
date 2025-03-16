using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject startRoom;
    public GameObject endRoom;
    public GameObject[] roomPrefabs;
    public GameObject[] hallwayPrefabs;
    public int dungeonLength = 10;

    private List<Transform> availablePoints = new List<Transform>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        GameObject currentRoom = Instantiate(startRoom, Vector3.zero, Quaternion.identity);
        Transform exitPoint = GetExitPoint(currentRoom);
        availablePoints.Add(exitPoint);

        for (int i = 0; i < dungeonLength; i++)
        {
            GameObject hallway = InstantiateRandom(hallwayPrefabs);
            PositionRoom(hallway);
            exitPoint = GetExitPoint(hallway);
            availablePoints.Add(exitPoint);

            GameObject room = InstantiateRandom(roomPrefabs);
            PositionRoom(room);
            exitPoint = GetExitPoint(room);
            availablePoints.Add(exitPoint);
        }

        GameObject finalRoom = Instantiate(endRoom);
        PositionRoom(finalRoom);
    }

    GameObject InstantiateRandom(GameObject[] prefabs)
    {
        return Instantiate(prefabs[Random.Range(0, prefabs.Length)]);
    }

    void PositionRoom(GameObject room)
    {
        if (availablePoints.Count == 0)
            return;

        Transform entryPoint = GetEntryPoint(room);
        Transform exitPoint = availablePoints[0];
        availablePoints.RemoveAt(0);

        room.transform.position = exitPoint.position - (entryPoint.position - room.transform.position);
    }

    Transform GetExitPoint(GameObject room)
    {
        Transform exit = room.transform.Find("ExitPoint");
        if (exit == null)
        {
            Debug.LogError("ExitPoint not found in " + room.name);
        }
        return exit;
    }

    Transform GetEntryPoint(GameObject room)
    {
        Transform entry = room.transform.Find("EntryPoint");
        if (entry == null)
        {
            Debug.LogError("EntryPoint not found in " + room.name);
        }
        return entry;
    }
}


