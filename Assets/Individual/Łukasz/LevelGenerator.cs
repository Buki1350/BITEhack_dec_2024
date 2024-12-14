using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] Vector3 StartRoomPosition = Vector3.zero;
    [SerializeField] private int roomNumber = 10;

    [SerializeField] private List<GameObject> rooms;
    [SerializeField] private List<GameObject> startRooms;
    [SerializeField] private List<GameObject> uniqueRooms;
    [SerializeField] private List<GameObject> optionalRooms;
    [SerializeField] private List<GameObject> uniqueOptionalRooms;
    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject testSphere;
    
    private List<GameObject> SquareRooms;
    private List<GameObject> LRooms;
    private List<GameObject> IRooms;

    private int currentRoomsCount = 0;
    
    int[,] levelMatrix;
    private int levelSize = 0;
    private int roomSize = 3;
    private int center = 0;
    
    private float inGameRoomSize = 150.0f;
    private float inGameRoomSpacing = 37.5f;

    private int createdRoomsCount = 0;
    
    bool isColliding;
    int deltaX;
    int deltaY;
    float initAngle = 0;
    int[,] roomMatrix = new int[1,1];
    bool isRoomMoving = false;
    int currentRoomX = 0;
    int currentRoomY = 0;

    private int borderLeft;
    private int borderRight;
    private int borderUp;
    private int borderDown;

    private GameObject[] bridgeGates = null;
    private int currentBridgeGateIndex = 0;
    
    void Start()
    {
        //2 sides for 2 (biggest room)
        levelSize = 1 + roomSize * roomNumber;
        levelMatrix = new int[levelSize, levelSize];
        center  = levelSize / 2;
        levelMatrix[center, center] = 2;
        
        borderLeft = borderRight = borderUp = borderDown = 1 + 1 + roomSize;
        
        
        //create id for every avaliable room
        
        
        
        //
        
        for (int i = 0; i < rooms.Count; i++)
        {
            switch (rooms[i].GetComponent<Room>().GetRoomType())
            {
                case "L":
                    LRooms.Add(rooms[i]);
                    break;
                case "I":
                    IRooms.Add(rooms[i]);
                    break;
                case "Square":
                    SquareRooms.Add(rooms[i]);
                    break;
            }
        }
        
        Instantiate(startRooms[Random.Range(0, startRooms.Count)],
            new Vector3(center * (inGameRoomSize + inGameRoomSpacing) - levelSize * (inGameRoomSize + inGameRoomSpacing)/2 + inGameRoomSize/2 + inGameRoomSpacing/2,
                 -inGameRoomSize/2,
                -center * (inGameRoomSize + inGameRoomSpacing) + levelSize * (inGameRoomSize + inGameRoomSpacing)/2 - inGameRoomSize/2 + inGameRoomSpacing/2), quaternion.identity);
    }
    
    void Update()
    {
        #region MatrixGeneration

        if (currentRoomsCount < roomNumber)
        {
            if (!isRoomMoving)
            {
                int randomBorder = Random.Range(0, 4);
                currentRoomX = 0;
                currentRoomY = 0;
                switch (randomBorder)
                {
                    case 0: //UP
                        currentRoomY = borderUp - roomSize;
                        currentRoomX = Random.Range(borderLeft - roomSize, borderRight + roomSize); //roomsize -1 for prevent "index out of range"
                        break;
                    case 1: //LEFT
                        currentRoomY = Random.Range(borderUp - roomSize, borderDown + roomSize);
                        currentRoomX = borderLeft - roomSize;
                        break;
                    case 2: //BOTTOM
                        currentRoomY = borderDown + roomSize;
                        currentRoomX = Random.Range(borderLeft - roomSize, borderRight + roomSize);
                        break;
                    case 3: //RIGHT
                        currentRoomY = Random.Range(borderUp - roomSize, borderDown + roomSize);
                        currentRoomX = borderRight + roomSize;
                        break;
                }

                GameObject currentRoom = rooms[Random.Range(0, rooms.Count)];
                roomMatrix = currentRoom.GetComponent<Room>().GetRoomTypeMatrix();
                

                deltaX = currentRoomX + 1 - center;
                deltaY = currentRoomY + 1 - center;
                initAngle = 0;
                if (deltaX != 0)
                    initAngle = Mathf.Atan2(Mathf.Abs(deltaY), Mathf.Abs(deltaX));

                isRoomMoving = true;
            }


            if (isRoomMoving)
            {
                deltaX = currentRoomX + 1 - center;
                deltaY = currentRoomY + 1 - center;
                
                int additionalX = 0;
                int additionalY = 0;
    
                // Calculate movement vector towards center
                float distance = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
    
                if (distance > 0)
                {
                    // // Normalize the direction
                    // float normalizedX = deltaX / distance;
                    // float normalizedY = deltaY / distance;
                    //
                    // // Determine primary movement direction based on which component is larger
                    // if (Mathf.Abs(normalizedX) > Mathf.Abs(normalizedY))
                    // {
                    //     additionalX = (normalizedX > 0) ? -1 : 1;
                    // }
                    // else
                    // {
                    //     additionalY = (normalizedY > 0) ? -1 : 1;
                    // }
                    //
                    // // Add randomization to prevent strict diagonal movement
                    // if (Random.value < 0.3f)
                    // {
                    //     if (additionalX == 0)
                    //     {
                    //         additionalX = (Random.value < 0.5f) ? -1 : 1;
                    //         additionalY = 0;
                    //     }
                    //     else if (additionalY == 0)
                    //     {
                    //         additionalY = (Random.value < 0.5f) ? -1 : 1;
                    //         additionalX = 0;
                    //     }
                    // }
                    
                    float alpha = Mathf.Atan2(Mathf.Abs(deltaY), Mathf.Abs(deltaX));

                    if (alpha > initAngle)
                        additionalY = 1;
                    else
                        additionalX = 1;

                    if (deltaX > 0)
                        additionalX *= -1;
                    if (deltaY > 0)
                        additionalY *= -1;
                    
                    Debug.Log(initAngle + "   " + alpha);
                }

                
                
                
                
                //Debug.Log("DX: " + deltaX + ", " + "DY: " + deltaY + ", angle: " + angle + ", AX" + additionalX + ", AY" + additionalY);
                
                
                for (int j = 0; j < roomSize; j++)
                {
                    for (int i = 0; i < roomSize; i++)
                    {
                        if (levelMatrix[currentRoomY + j + 1, currentRoomX + i] * roomMatrix[j, i] < 0 ||
                            levelMatrix[currentRoomY + j - 1, currentRoomX + i] * roomMatrix[j, i] < 0 ||
                            levelMatrix[currentRoomY + j, currentRoomX + i + 1] * roomMatrix[j, i] < 0 ||
                            levelMatrix[currentRoomY + j, currentRoomX + i - 1] * roomMatrix[j, i] < 0)
                        {
                            isColliding = true;
                        }
                    }
                }

                if (!isColliding)
                {
                    currentRoomX += additionalX;
                    currentRoomY += additionalY;
                }
                else
                {
                    for (int j = 0; j < roomSize; j++)
                    {
                        for (int i = 0; i < roomSize; i++)
                        {
                            if (roomMatrix[j, i] != 0)
                            {
                                levelMatrix[currentRoomY + j, currentRoomX + i] = -roomMatrix[j, i];
                                
                                switch (levelMatrix[currentRoomY + j, currentRoomX + i])
                                {
                                    case 3:
                                        Instantiate(SquareRooms[Random.Range(0, SquareRooms.Count)],
                                            new Vector3((currentRoomX + i) * (inGameRoomSize + inGameRoomSpacing) - levelSize * (inGameRoomSize + inGameRoomSpacing)/2 + inGameRoomSize/2 + inGameRoomSpacing/2,
                                                -inGameRoomSize/2,
                                                -(currentRoomY + j) * (inGameRoomSize + inGameRoomSpacing) + levelSize * (inGameRoomSize + inGameRoomSpacing)/2 - inGameRoomSize/2 + inGameRoomSpacing/2), quaternion.identity); 
                                        break;
                                    case 4:
                                        Instantiate(LRooms[Random.Range(0, LRooms.Count)],
                                            new Vector3((currentRoomX + i) * (inGameRoomSize + inGameRoomSpacing) - levelSize * (inGameRoomSize + inGameRoomSpacing)/2 + inGameRoomSize/2 + inGameRoomSpacing/2,
                                                -inGameRoomSize/2,
                                                -(currentRoomY + j) * (inGameRoomSize + inGameRoomSpacing) + levelSize * (inGameRoomSize + inGameRoomSpacing)/2 - inGameRoomSize/2 + inGameRoomSpacing/2), quaternion.identity); 
                                        break;
                                    case 5:
                                        Instantiate(IRooms[Random.Range(0, IRooms.Count)],
                                            new Vector3((currentRoomX + i) * (inGameRoomSize + inGameRoomSpacing) - levelSize * (inGameRoomSize + inGameRoomSpacing)/2 + inGameRoomSize/2 + inGameRoomSpacing/2, 
                                                -inGameRoomSize/2,
                                                -(currentRoomY + j) * (inGameRoomSize + inGameRoomSpacing) + levelSize * (inGameRoomSize + inGameRoomSpacing)/2 - inGameRoomSize/2 + inGameRoomSpacing/2), quaternion.identity);
                                        break;
                                }

                                createdRoomsCount++;
                            }
                        }
                    }
                    
                    bool borderSet = false;
                    for (int _j = 0; _j < levelSize || !borderSet; _j++)
                    {
                        for (int _i = 0; _i < levelSize; _i++)
                        {
                            if (levelMatrix[_j, _i] != 0)
                            {
                                borderDown = _j;
                                borderSet = true;
                            }
                        }
                    }
                    
                    borderSet = false;
                    for (int _j = levelSize - 1; _j >= 0 || !borderSet; _j--)
                    {
                        for (int _i = 0; _i < levelSize; _i++)
                        {
                            if (levelMatrix[_j, _i] != 0)
                            {
                                borderUp = _j;
                                borderSet = true;
                            }
                        }
                    }
                    
                    borderSet = false;
                    for (int _i = 0; _i < levelSize || !borderSet; _i++)
                    {
                        for (int _j = 0; _j < levelSize; _j++)
                        {
                            if (levelMatrix[_j, _i] != 0)
                            {
                                borderRight = _i;
                                borderSet = true;
                            }
                        }
                    }
                    
                    borderSet = false;
                    for (int _i = levelSize - 1; _i >= 0 || !borderSet; _i--)
                    {
                        for (int _j = 0; _j < levelSize; _j++)
                        {
                            if (levelMatrix[_j, _i] != 0)
                            {
                                borderLeft = _i;
                                borderSet = true;
                            }
                        }
                    }
                    
                    isRoomMoving = false;
                    isColliding = false;
                    currentRoomsCount++;
                    Debug.Log("Creating rooms: " + currentRoomsCount + " / " + roomNumber);
                }

                // for (int j = 0; j < levelSize; j++)
                // {
                //     string line = "";
                //     for (int i = 0; i < levelSize; i++)
                //     {
                //         if (i >= currentRoomX && i < currentRoomX + roomSize && j >= currentRoomY &&
                //             j < currentRoomY + roomSize)
                //             if (levelMatrix[j, i] != 0)
                //                 line += levelMatrix[j, i];
                //             else
                //                 line += roomMatrix[j - currentRoomY, i - currentRoomX];
                //             
                //         else if (levelMatrix[j, i] == 0)
                //             line += " ";
                //         else
                //             line += levelMatrix[j, i].ToString();
                //     }
                //     Debug.Log(line+"\n");
                // }
                // Debug.Log("\n\n\n");
                
                
            }

        }
        #endregion
        
        if (currentRoomsCount == roomNumber)
        {
            
            if (bridgeGates == null)
                bridgeGates = GameObject.FindGameObjectsWithTag("BridgeGate");
            if (currentBridgeGateIndex < bridgeGates.Length)
            {
                Debug.Log("Current bridge gate index: " + currentBridgeGateIndex + ", total bridge gate number: " + bridgeGates.Length);
                if (bridgeGates[currentBridgeGateIndex].GetComponent<BridgeGate>().Connected == false)
                {
                    bool find = false;
                    int i = 0;
                    for (i = 0; i < inGameRoomSpacing * 2 && !find; i++)
                    {
                        Collider[] hitColliders = Physics.OverlapSphere(bridgeGates[currentBridgeGateIndex].transform.position, i, LayerMask.GetMask("BridgeGate"));
                        Debug.Log(hitColliders.Length);
                        foreach (Collider collider in hitColliders)
                        {
                            if (!find)
                            {
                                if (collider.gameObject != bridgeGates[currentBridgeGateIndex] && collider.gameObject.CompareTag("BridgeGate") && collider.transform.parent != bridgeGates[currentBridgeGateIndex].transform.parent)
                                {
                                    find = true;
                                    var secondBridgeGate = collider.gameObject;

                                    // Calculate the position for the bridge (midpoint between both gates)
                                    Vector3 bridgePosition = (bridgeGates[currentBridgeGateIndex].transform.position + secondBridgeGate.transform.position) / 2;

                                    // Calculate the rotation for the bridge (facing from first gate to second gate)
                                    Quaternion bridgeRotation = Quaternion.LookRotation(secondBridgeGate.transform.position - bridgeGates[currentBridgeGateIndex].transform.position);

                                    // Instantiate the bridge prefab at the correct position and rotation
                                    GameObject newBridge = Instantiate(bridge, bridgePosition, bridgeRotation);
                                    newBridge.transform.position -= new Vector3(0,
                                        newBridge.GetComponentInChildren<BoxCollider>().bounds.size.y/2, 0);

                                    bridgeGates[currentBridgeGateIndex].GetComponent<BridgeGate>().Connected = true;
                                    secondBridgeGate.GetComponent<BridgeGate>().Connected = true;
                                    
                                }
                            }
                        }
                    }
                    bridgeGates[currentBridgeGateIndex].GetComponent<BridgeGate>().Connected = true;
                }
                currentBridgeGateIndex++;
            }
        }
    }
}
