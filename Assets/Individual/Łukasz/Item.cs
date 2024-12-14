using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class Item : MonoBehaviour
{
    [SerializeField] public Texture2D icon;
    void Start()                                    
    {
        gameObject.layer = LayerMask.NameToLayer("Pickable");
        if (gameObject.GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
            gameObject.GetComponent<BoxCollider>().includeLayers = LayerMask.GetMask("PickupCollider");
        }
        if (gameObject.GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
            gameObject.GetComponent<Rigidbody>().includeLayers = 0;
        }
    }
}
