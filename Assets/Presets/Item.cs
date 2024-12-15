using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] public Texture2D icon;

    [Header("Floating Settings")]
    [SerializeField] private float floatAmplitude = 0.5f; // Amplituda unoszenia
    [SerializeField] private float floatFrequency = 1f;   // Częstotliwość unoszenia
    [SerializeField] private float rotationSpeed = 50f;   // Prędkość obracania

    private Vector3 startPosition;

    void Start()
    {
        // Ustawienie warstwy
        gameObject.layer = LayerMask.NameToLayer("Pickable");

        // Dodanie komponentu BoxCollider, jeśli go nie ma
        if (gameObject.GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        // Dodanie komponentu Rigidbody, jeśli go nie ma
        if (gameObject.GetComponent<Rigidbody>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Obiekt nie będzie podlegał fizyce
        }

        // Zapamiętanie początkowej pozycji
        startPosition = transform.position;
    }

    void Update()
    {
        // Efekt unoszenia
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Obracanie obiektu
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
    }
}