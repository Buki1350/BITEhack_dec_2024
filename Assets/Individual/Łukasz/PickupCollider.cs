using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PickupCollider : MonoBehaviour
{
    public PlayerEquipment playerEquipment;
    public int pickableLayer;
    private List<GameObject> pickables;
    [SerializeField] public Material outlineMaterial;

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
    }
    
    private void Update()
    {
        
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
