using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class doors : MonoBehaviour
{
    [SerializeField] private GameObject leftDoorOrigin;
    [SerializeField] private GameObject rightDoorOrigin;
    [HideInInspector] public bool openDoors = false;

    private float timeForOpenDoors = 1.0f;

    private float currentTime = 0;

    private float additionalRotation = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (openDoors)
            OpenDoors();
    }

    void OpenDoors()
    {
        if (currentTime < timeForOpenDoors)
        {
            additionalRotation = 90 * (currentTime / timeForOpenDoors);
            leftDoorOrigin.transform.localEulerAngles = new Vector3(leftDoorOrigin.transform.localEulerAngles.x,
                180-additionalRotation, leftDoorOrigin.transform.localEulerAngles.z);
            rightDoorOrigin.transform.localEulerAngles = new Vector3(leftDoorOrigin.transform.localEulerAngles.x,
                -180+additionalRotation, leftDoorOrigin.transform.localEulerAngles.z);
            currentTime += Time.deltaTime;
        }
    }
}
