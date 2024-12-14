using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class Item : MonoBehaviour
{
    [SerializeField] public Texture2D icon;
    void Start()                                    
    {
        gameObject.layer = LayerMask.NameToLayer("Pickable");
    }
}
