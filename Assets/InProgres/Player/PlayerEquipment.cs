using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] float pickupRadius = 5f;
    [SerializeField] public KeyCode pickupKey = KeyCode.E;
    [SerializeField] public KeyCode throwKey = KeyCode.Q;
    [SerializeField] public GameObject playerModel;
    [SerializeField] public GameObject playerEqipmentInGameSpace;
    [SerializeField] public List<Image> uiItemHolders = new List<Image>();
    [SerializeField] public GameObject currentItemBorder;
    [HideInInspector] public List<GameObject> equipment = new List<GameObject>();
    [HideInInspector] public List<bool> itemUiCreated = new List<bool>(); 
    
    private int currentHoldItemIndex = 0;
    public Sprite defaultItemHolderSprite; 
    void Start()
    {
        defaultItemHolderSprite = uiItemHolders[0].GetComponent<Image>().sprite;
        
        for (int i = 0; i < uiItemHolders.Count; i++)
        {
            equipment.Add(null);
            itemUiCreated.Add(false);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            currentHoldItemIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            currentHoldItemIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            currentHoldItemIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            currentHoldItemIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            currentHoldItemIndex = 4;
        
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        if (scrollValue < 0) // Scroll w górę
        {
            currentHoldItemIndex++;
            if (currentHoldItemIndex >= equipment.Count)
                currentHoldItemIndex = 0; // Powrót na początek
        }
        else if (scrollValue > 0) // Scroll w dół
        {
            currentHoldItemIndex--;
            if (currentHoldItemIndex < 0)
                currentHoldItemIndex = equipment.Count - 1; // Powrót na koniec
        }
        
        currentItemBorder.transform.position = uiItemHolders[currentHoldItemIndex].transform.position;
        
        for (int i = 0; i < equipment.Count; i++)
        {
            if (!itemUiCreated[i] && equipment[i] != null)
            {
                Texture2D itemIcon = equipment[i].GetComponent<Item>().icon;
                Sprite newSprite = Sprite.Create(
                    itemIcon,
                    new Rect(0, 0, itemIcon.width, itemIcon.height),
                    new Vector2(0.5f, 0.5f)
                );
                uiItemHolders[i].GetComponent<Image>().sprite = newSprite;
                itemUiCreated[i] = true;
            }
        }

        if (Input.GetKeyDown(throwKey))
        {
            if (equipment[currentHoldItemIndex] != null)
            {
                GameObject thrownItem = Instantiate(equipment[currentHoldItemIndex], playerModel.transform.position,
                    Quaternion.identity);
                thrownItem.name = equipment[currentHoldItemIndex].name;
                thrownItem.SetActive(true);
                thrownItem.AddComponent<Rigidbody>();
                thrownItem.GetComponent<Rigidbody>().AddForce(playerModel.transform.forward * 200);
                uiItemHolders[currentHoldItemIndex].sprite = defaultItemHolderSprite;
                Destroy(equipment[currentHoldItemIndex]);
                equipment[currentHoldItemIndex] = null;
                itemUiCreated[currentHoldItemIndex] = false;
            }
        }
    }
}