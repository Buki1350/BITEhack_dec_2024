using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum RoomTypes
{
    Square,
    L,
    I
}


public class Room : MonoBehaviour
{
    [HideInInspector] public int roomID = -1;
    
    //1 for empty space
    //2 for starting room
    //3 for square
    //4 for L
    //5 for I
    private int[,] squareRoomMatrix =
    {
        {0, 0, 0 },
        {0, -2, 0 },
        {0, 0, 0 },
    };
    private int[,] LRoomMatrix =
    {
        {0, -1, 0 },
        { 0, -2, -1 },
        { 0, 0, 0 }
    };

    private int[,] IRoomMatrix =
    {
        {0, 0, 0},
        { 0, -2, -1 },
        { 0, 0, 0 }
    };
    
    
    [SerializeField] private RoomTypes roomType;

    private GameObject player;
    [HideInInspector] public int[,] roomMatrix;

    private bool isPlayerAbove = false;
    private bool isRoomCleared = true;
    private int bridgeCountInThisRoom = 0;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (player != null)
        {
            isPlayerAbove = false;
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.down, out hit, Mathf.Infinity,
                    LayerMask.GetMask("Room")))
            {
                if (hit.collider.transform.IsChildOf(transform) || hit.collider.gameObject == gameObject)
                {
                    isPlayerAbove = true;
                }
            }
        }
    }
    
    public int[,] GetRoomTypeMatrix()
    {
        if (roomType == RoomTypes.L)
            return LRoomMatrix;
        else if (roomType == RoomTypes.I)
            return IRoomMatrix;
        else
            return squareRoomMatrix;
    }
    
    public string GetRoomType()
    {
        if (roomType == RoomTypes.L)
            return "L";
        else if (roomType == RoomTypes.I)
            return "I";
        else
            return "Square";
    }
}
