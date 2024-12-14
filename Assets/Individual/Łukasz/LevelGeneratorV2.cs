using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Individual.Łukasz
{
    public class LevelGeneratorV2 : MonoBehaviour
    {
        [SerializeField] private int numberOfStandardRooms = 10;

        [SerializeField] private List<GameObject> rooms;
        [SerializeField] private List<GameObject> startRooms;
        [SerializeField] private List<GameObject> uniqueRooms;
        [SerializeField] private List<GameObject> optionalRooms;
        [SerializeField] private List<GameObject> uniqueOptionalRooms;
        [SerializeField] private GameObject bridge;
    
        private List<GameObject> _uniqueRoomsToCreate;

        private int _currentRoomsCount = 0;
    
        private int[,] _levelMatrix;
        private int _levelSize = 0;
        private int _roomSize = 3;
        private int _center = 0;
    
        private float _inGameRoomSize = 150.0f;
        private float _inGameRoomSpacing = 37.5f;

        private int _createdRoomsCount = 0;
    
        private bool _isColliding;
        private int _deltaX;
        private int _deltaY;
        private float _initAngle = 0;
        private int[,] _roomMatrix = new int[1,1];
        private bool _isRoomMoving = false;
        private int _currentRoomX = 0;
        private int _currentRoomY = 0;

        private int borderLeft;
        private int borderRight;
        private int borderUp;
        private int borderDown;

        private GameObject[] bridgeGates = null;
        private int currentBridgeGateIndex = 0;
    
        void Start()
        {
            //2 sides for 2 (biggest room)
            _levelSize = 1 + _roomSize * (numberOfStandardRooms + uniqueRooms.Count * 2);
            _levelMatrix = new int[_levelSize, _levelSize];
            _center  = _levelSize / 2;
        
            borderLeft = borderRight = borderUp = borderDown = 1 + 1 + _roomSize;

            #region assign ID for every room
        
            //create id for every avaliable room
            int IDs = 2;
            foreach (GameObject room in rooms)
            {
                Room roomComponent = room.GetComponent<Room>();
                roomComponent.roomID = IDs;
                int[,] newRoomMatrix = roomComponent.GetRoomTypeMatrix();
                for (int row = 0; row < newRoomMatrix.GetLength(0); row++)
                {
                    for (int col = 0; col < newRoomMatrix.GetLength(1); col++)
                    {
                        if (newRoomMatrix[row, col] == -2)
                        {
                            newRoomMatrix[row, col] = -IDs;
                        }
                    }
                }
                roomComponent.roomMatrix = newRoomMatrix;
                IDs++;
            }
            foreach (GameObject room in startRooms)
            {
                Room roomComponent = room.GetComponent<Room>();
                roomComponent.roomID = IDs;
                int[,] newRoomMatrix = roomComponent.GetRoomTypeMatrix();
                for (int row = 0; row < newRoomMatrix.GetLength(0); row++)
                {
                    for (int col = 0; col < newRoomMatrix.GetLength(1); col++)
                    {
                        if (newRoomMatrix[row, col] == -2)
                        {
                            newRoomMatrix[row, col] = -IDs;
                        }
                    }
                }
                roomComponent.roomMatrix = newRoomMatrix;
                IDs++;
            }
            foreach (GameObject room in uniqueRooms)
            {
                Room roomComponent = room.GetComponent<Room>();
                roomComponent.roomID = IDs;
                int[,] newRoomMatrix = roomComponent.GetRoomTypeMatrix();
                for (int row = 0; row < newRoomMatrix.GetLength(0); row++)
                {
                    for (int col = 0; col < newRoomMatrix.GetLength(1); col++)
                    {
                        if (newRoomMatrix[row, col] == -2)
                        {
                            newRoomMatrix[row, col] = -IDs;
                        }
                    }
                }
                roomComponent.roomMatrix = newRoomMatrix;
                IDs++;
            }
            foreach (GameObject room in optionalRooms)
            {
                Room roomComponent = room.GetComponent<Room>();
                roomComponent.roomID = IDs;
                int[,] newRoomMatrix = roomComponent.GetRoomTypeMatrix();
                for (int row = 0; row < newRoomMatrix.GetLength(0); row++)
                {
                    for (int col = 0; col < newRoomMatrix.GetLength(1); col++)
                    {
                        if (newRoomMatrix[row, col] == -2)
                        {
                            newRoomMatrix[row, col] = -IDs;
                        }
                    }
                }
                roomComponent.roomMatrix = newRoomMatrix;
                IDs++;
            }
            foreach (GameObject room in uniqueOptionalRooms)
            {
                Room roomComponent = room.GetComponent<Room>();
                roomComponent.roomID = IDs;
                int[,] newRoomMatrix = roomComponent.GetRoomTypeMatrix();
                for (int row = 0; row < newRoomMatrix.GetLength(0); row++)
                {
                    for (int col = 0; col < newRoomMatrix.GetLength(1); col++)
                    {
                        if (newRoomMatrix[row, col] == -2)
                        {
                            newRoomMatrix[row, col] = -IDs;
                        }
                    }
                }
                roomComponent.roomMatrix = newRoomMatrix;
                IDs++;
            }
            //
            #endregion

            _uniqueRoomsToCreate = new List<GameObject>();
            foreach (var t in uniqueRooms)
                _uniqueRoomsToCreate.Add(t);
            // _uniqueOptionalRoomsToCreate = new List<GameObject>();
            // foreach (var t in uniqueOptionalRooms)
            //     _uniqueOptionalRoomsToCreate.Add(t);
        
            numberOfStandardRooms += uniqueRooms.Count;


            GameObject selectedStartRoom = startRooms[Random.Range(0, startRooms.Count)];
            _levelMatrix[_center, _center] = selectedStartRoom.GetComponent<Room>().roomID;
            Instantiate(selectedStartRoom,
                new Vector3(_center * (_inGameRoomSize + _inGameRoomSpacing) - _levelSize * (_inGameRoomSize + _inGameRoomSpacing)/2 + _inGameRoomSize/2 + _inGameRoomSpacing/2,
                    -_inGameRoomSize/2,
                    -_center * (_inGameRoomSize + _inGameRoomSpacing) + _levelSize * (_inGameRoomSize + _inGameRoomSpacing)/2 - _inGameRoomSize/2 + _inGameRoomSpacing/2), quaternion.identity);
        }
    
        void Update()
        {
            //Debug.Log(uniqueRooms.Count + "     " + _uniqueRoomsToCreate.Count);
            #region MatrixGeneration

            if (_currentRoomsCount < numberOfStandardRooms)
            {
                if (!_isRoomMoving)
                {
                    int randomBorder = Random.Range(0, 4);
                    _currentRoomX = 0;
                    _currentRoomY = 0;
                    switch (randomBorder)
                    {
                        case 0: //UP
                            _currentRoomY = borderUp - _roomSize;
                            _currentRoomX = Random.Range(borderLeft - _roomSize, borderRight + _roomSize); //roomsize -1 for prevent "index out of range"
                            break;
                        case 1: //LEFT
                            _currentRoomY = Random.Range(borderUp - _roomSize, borderDown + _roomSize);
                            _currentRoomX = borderLeft - _roomSize;
                            break;
                        case 2: //BOTTOM
                            _currentRoomY = borderDown + _roomSize;
                            _currentRoomX = Random.Range(borderLeft - _roomSize, borderRight + _roomSize);
                            break;
                        case 3: //RIGHT
                            _currentRoomY = Random.Range(borderUp - _roomSize, borderDown + _roomSize);
                            _currentRoomX = borderRight + _roomSize;
                            break;
                    }

                    GameObject currentRoom;
                    if (_uniqueRoomsToCreate.Count != 0 && Random.Range(_currentRoomsCount, numberOfStandardRooms) == _currentRoomsCount)
                    {
                        int randomUniqueRoomIndex = Random.Range(0, _uniqueRoomsToCreate.Count);
                        currentRoom = _uniqueRoomsToCreate[randomUniqueRoomIndex];
                        _uniqueRoomsToCreate.RemoveAt(randomUniqueRoomIndex);

                    }
                    else
                        currentRoom = rooms[Random.Range(0, rooms.Count)];
                
                    _roomMatrix = currentRoom.GetComponent<Room>().GetRoomTypeMatrix();
                

                    _deltaX = _currentRoomX + 1 - _center;
                    _deltaY = _currentRoomY + 1 - _center;
                    _initAngle = 0;
                    if (_deltaX != 0)
                        _initAngle = Mathf.Atan2(Mathf.Abs(_deltaY), Mathf.Abs(_deltaX));

                    _isRoomMoving = true;
                }


                if (_isRoomMoving)
                {
                    _deltaX = _currentRoomX + 1 - _center;
                    _deltaY = _currentRoomY + 1 - _center;
                
                    int additionalX = 0;
                    int additionalY = 0;
    
                    // Calculate movement vector towards center
                    float distance = Mathf.Sqrt(_deltaX * _deltaX + _deltaY * _deltaY);
    
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
                    
                        float alpha = Mathf.Atan2(Mathf.Abs(_deltaY), Mathf.Abs(_deltaX));

                        if (alpha > _initAngle)
                            additionalY = 1;
                        else
                            additionalX = 1;

                        if (_deltaX > 0)
                            additionalX *= -1;
                        if (_deltaY > 0)
                            additionalY *= -1;
                    
                        // Debug.Log(initAngle + "   " + alpha);
                    }

                
                
                
                
                    //Debug.Log("DX: " + deltaX + ", " + "DY: " + deltaY + ", angle: " + angle + ", AX" + additionalX + ", AY" + additionalY);
                
                
                    for (int j = 0; j < _roomSize; j++)
                    {
                        for (int i = 0; i < _roomSize; i++)
                        {
                            if (_levelMatrix[_currentRoomY + j + 1, _currentRoomX + i] * _roomMatrix[j, i] < 0 ||
                                _levelMatrix[_currentRoomY + j - 1, _currentRoomX + i] * _roomMatrix[j, i] < 0 ||
                                _levelMatrix[_currentRoomY + j, _currentRoomX + i + 1] * _roomMatrix[j, i] < 0 ||
                                _levelMatrix[_currentRoomY + j, _currentRoomX + i - 1] * _roomMatrix[j, i] < 0)
                            {
                                _isColliding = true;
                            }
                        }
                    }

                    if (!_isColliding)
                    {
                        _currentRoomX += additionalX;
                        _currentRoomY += additionalY;
                    }
                    else
                    {
                        for (int j = 0; j < _roomSize; j++)
                        {
                            for (int i = 0; i < _roomSize; i++)
                            {
                                if (_roomMatrix[j, i] != 0)
                                {
                                    _levelMatrix[_currentRoomY + j, _currentRoomX + i] = -_roomMatrix[j, i];

                                    bool created = false;
                                    for (int roomIndex = 0; roomIndex < rooms.Count && !created; roomIndex++)
                                    {
                                        if (rooms[roomIndex].GetComponent<Room>().roomID ==
                                            _levelMatrix[_currentRoomY + j, _currentRoomX + i])
                                        {
                                            Instantiate(rooms[roomIndex],
                                                new Vector3((_currentRoomX + i) * (_inGameRoomSize + _inGameRoomSpacing) - _levelSize * (_inGameRoomSize + _inGameRoomSpacing)/2 + _inGameRoomSize/2 + _inGameRoomSpacing/2,
                                                    -_inGameRoomSize/2,
                                                    -(_currentRoomY + j) * (_inGameRoomSize + _inGameRoomSpacing) + _levelSize * (_inGameRoomSize + _inGameRoomSpacing)/2 - _inGameRoomSize/2 + _inGameRoomSpacing/2), quaternion.identity);

                                            created = true;
                                        }
                                    }

                                    if (!created)
                                    {
                                        for (int roomIndex = 0; roomIndex < uniqueRooms.Count && !created; roomIndex++)
                                        {
                                            if (uniqueRooms[roomIndex].GetComponent<Room>().roomID ==
                                                _levelMatrix[_currentRoomY + j, _currentRoomX + i])
                                            {
                                                Instantiate(uniqueRooms[roomIndex],
                                                    new Vector3((_currentRoomX + i) * (_inGameRoomSize + _inGameRoomSpacing) - _levelSize * (_inGameRoomSize + _inGameRoomSpacing)/2 + _inGameRoomSize/2 + _inGameRoomSpacing/2,
                                                        -_inGameRoomSize/2,
                                                        -(_currentRoomY + j) * (_inGameRoomSize + _inGameRoomSpacing) + _levelSize * (_inGameRoomSize + _inGameRoomSpacing)/2 - _inGameRoomSize/2 + _inGameRoomSpacing/2), quaternion.identity);

                                                created = true;
                                            }
                                        }
                                    }
                                
                                    // switch (levelMatrix[currentRoomY + j, currentRoomX + i])
                                    // {
                                    //     case 3:
                                    //         Instantiate(SquareRooms[Random.Range(0, SquareRooms.Count)],
                                    //             new Vector3((currentRoomX + i) * (inGameRoomSize + inGameRoomSpacing) - levelSize * (inGameRoomSize + inGameRoomSpacing)/2 + inGameRoomSize/2 + inGameRoomSpacing/2,
                                    //                 -inGameRoomSize/2,
                                    //                 -(currentRoomY + j) * (inGameRoomSize + inGameRoomSpacing) + levelSize * (inGameRoomSize + inGameRoomSpacing)/2 - inGameRoomSize/2 + inGameRoomSpacing/2), quaternion.identity); 
                                    //         break;
                                    //     case 4:
                                    //         Instantiate(LRooms[Random.Range(0, LRooms.Count)],
                                    //             new Vector3((currentRoomX + i) * (inGameRoomSize + inGameRoomSpacing) - levelSize * (inGameRoomSize + inGameRoomSpacing)/2 + inGameRoomSize/2 + inGameRoomSpacing/2,
                                    //                 -inGameRoomSize/2,
                                    //                 -(currentRoomY + j) * (inGameRoomSize + inGameRoomSpacing) + levelSize * (inGameRoomSize + inGameRoomSpacing)/2 - inGameRoomSize/2 + inGameRoomSpacing/2), quaternion.identity); 
                                    //         break;
                                    //     case 5:
                                    //         Instantiate(IRooms[Random.Range(0, IRooms.Count)],
                                    //             new Vector3((currentRoomX + i) * (inGameRoomSize + inGameRoomSpacing) - levelSize * (inGameRoomSize + inGameRoomSpacing)/2 + inGameRoomSize/2 + inGameRoomSpacing/2, 
                                    //                 -inGameRoomSize/2,
                                    //                 -(currentRoomY + j) * (inGameRoomSize + inGameRoomSpacing) + levelSize * (inGameRoomSize + inGameRoomSpacing)/2 - inGameRoomSize/2 + inGameRoomSpacing/2), quaternion.identity);
                                    //         break;
                                    // }

                                    _createdRoomsCount++;
                                }
                            }
                        }
                    
                        bool borderSet = false;
                        for (int _j = 0; _j < _levelSize || !borderSet; _j++)
                        {
                            for (int _i = 0; _i < _levelSize; _i++)
                            {
                                if (_levelMatrix[_j, _i] != 0)
                                {
                                    borderDown = _j;
                                    borderSet = true;
                                }
                            }
                        }
                    
                        borderSet = false;
                        for (int _j = _levelSize - 1; _j >= 0 || !borderSet; _j--)
                        {
                            for (int _i = 0; _i < _levelSize; _i++)
                            {
                                if (_levelMatrix[_j, _i] != 0)
                                {
                                    borderUp = _j;
                                    borderSet = true;
                                }
                            }
                        }
                    
                        borderSet = false;
                        for (int _i = 0; _i < _levelSize || !borderSet; _i++)
                        {
                            for (int _j = 0; _j < _levelSize; _j++)
                            {
                                if (_levelMatrix[_j, _i] != 0)
                                {
                                    borderRight = _i;
                                    borderSet = true;
                                }
                            }
                        }
                    
                        borderSet = false;
                        for (int _i = _levelSize - 1; _i >= 0 || !borderSet; _i--)
                        {
                            for (int _j = 0; _j < _levelSize; _j++)
                            {
                                if (_levelMatrix[_j, _i] != 0)
                                {
                                    borderLeft = _i;
                                    borderSet = true;
                                }
                            }
                        }
                    
                        _isRoomMoving = false;
                        _isColliding = false;
                        _currentRoomsCount++;
                    }

                    // for (int j = 0; j < _levelSize; j++)
                    // {
                    //     string line = "";
                    //     for (int i = 0; i < _levelSize; i++)
                    //     {
                    //         if (i >= _currentRoomX && i < _currentRoomX + _roomSize && j >= _currentRoomY &&
                    //             j < _currentRoomY + _roomSize)
                    //             if (_levelMatrix[j, i] != 0)
                    //                 line += _levelMatrix[j, i];
                    //             else
                    //                 line += _roomMatrix[j - _currentRoomY, i - _currentRoomX];
                    //         
                    //         else if (_levelMatrix[j, i] == 0)
                    //             line += " ";
                    //         else
                    //             line += _levelMatrix[j, i].ToString();
                    //     }
                    //     Debug.Log(line+"\n");
                    // }
                    // Debug.Log("\n\n\n");
                
                
                }

            }
            #endregion

            #region BridgeMaking

            Debug.Log(_currentRoomsCount + "   " + numberOfStandardRooms);
            if (_currentRoomsCount == numberOfStandardRooms)
            {
            
                if (bridgeGates == null)
                    bridgeGates = GameObject.FindGameObjectsWithTag("BridgeGate");
                if (currentBridgeGateIndex < bridgeGates.Length)
                {
                    if (bridgeGates[currentBridgeGateIndex].GetComponent<BridgeGate>().Connected == false)
                    {
                        bool find = false;
                        int i = 0;
                        for (i = 0; i < _inGameRoomSpacing * 2 && !find; i++)
                        {
                            Collider[] hitColliders = Physics.OverlapSphere(bridgeGates[currentBridgeGateIndex].transform.position, i, LayerMask.GetMask("BridgeGate"));
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
            #endregion
        }
    }
}
