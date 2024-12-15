using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PickupCollider : MonoBehaviour
{
    public PlayerEquipment playerEquipment;
    public int pickableLayer;
    private List<GameObject> pickables;
    [SerializeField] public Material outlineMaterial;
    [SerializeField] private TextMeshProUGUI itemInfo;

    private GameObject doors;

    void Start()
    {
        pickableLayer = LayerMask.NameToLayer("Pickable");
        pickables = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == pickableLayer)
        {
            pickables.Add(other.gameObject);
            Material[] currentMaterials = other.gameObject.GetComponent<MeshRenderer>().materials;
            Material[] newMaterials = new Material[currentMaterials.Length + 1];
            for (int i = 0; i < currentMaterials.Length; i++)
                newMaterials[i] = currentMaterials[i];
            newMaterials[newMaterials.Length - 1] = outlineMaterial;
            other.gameObject.GetComponent<MeshRenderer>().materials = newMaterials;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Doors"))
        {
            
            doors = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == pickableLayer)
        {
            pickables.Remove(other.gameObject);
            
            Material[] currentMaterials = other.gameObject.GetComponent<MeshRenderer>().materials;
            Material[] newMaterials = new Material[currentMaterials.Length - 1];
            for (int i = 0; i < newMaterials.Length; i++)
                newMaterials[i] = currentMaterials[i];
            other.gameObject.GetComponent<MeshRenderer>().materials = newMaterials;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Doors"))
        {
            doors = null;
        }
    }
    
    private void Update()
    {
        if (pickables.Count != 0)
        {
            itemInfo.text = "Naciśnij E by podnieść: " + pickables[0].name;
        }
        else
        {
            itemInfo.text = "";
        }

        if (doors != null)
        {
            bool foundKey = false;
            for (int i = 0; i < playerEquipment.equipment.Count; i++)
            {
                if (playerEquipment.equipment[i] != null && playerEquipment.equipment[i].CompareTag("Key"))
                    foundKey = true;
            }

            if (!foundKey)
            {
                itemInfo.text = "Klucz do tych drzwi znajdywał się w tym miejscu, jednak dawno obrócił się w pył...";
            }
            else
            {
                itemInfo.text = "Wciśnij F aby użyć klucza";
                if (Input.GetKeyDown(KeyCode.F))
                {
                    doors.GetComponent<doors>().openDoors = true;
                    for (int i = 0; i < playerEquipment.equipment.Count; i++)
                    {
                        if (playerEquipment.equipment[i] != null && playerEquipment.equipment[i].CompareTag("Key"))
                        {
                            playerEquipment.equipment[i] = null;
                            playerEquipment.uiItemHolders[i].sprite = playerEquipment.defaultItemHolderSprite;
                        }
                    }
                }
            }
        }
        
        
        
        if (Input.GetKeyDown(playerEquipment.pickupKey))
        {
            bool picked = false;
            for (int i = 0; i < playerEquipment.equipment.Count && !picked; i++)
            {
                if (playerEquipment.equipment[i] == null && pickables.Count > 0)
                {
                    float minDistance = float.MaxValue;
                    GameObject itemToPick = null;
                    for (int j = 0; j < pickables.Count; j++)
                    {
                        if (Vector3.Distance(transform.position, pickables[j].transform.position) < minDistance)
                        {
                            itemToPick = pickables[j];
                            minDistance = Vector3.Distance(transform.position, pickables[j].transform.position);
                        }

                        GameObject newEquippedItem = Instantiate(itemToPick,
                            playerEquipment.playerEqipmentInGameSpace.transform, false);
                        newEquippedItem.name = itemToPick.name;
                        newEquippedItem.SetActive(false);
                        playerEquipment.equipment[i] = newEquippedItem;
                        pickables.Remove(itemToPick);
                        Destroy(itemToPick);
                        picked = true;
                    }
                }
            }
        }
    }
    }
