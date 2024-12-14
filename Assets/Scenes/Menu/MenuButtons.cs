using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private Image blackShadeOutImage;
    [SerializeField] private float shadeTime;
    
    private bool startButtonPressed = false;
    public void StartButton()
    {
        startButtonPressed = true;
    }

    private void Update()
    {
        if (startButtonPressed)
        {
            if (blackShadeOutImage.color.a < 1)
            {
                Debug.Log(blackShadeOutImage.color.a);
                Color newColor = blackShadeOutImage.color;
                newColor.a += Time.deltaTime * shadeTime;
                blackShadeOutImage.color = newColor;
            }
            else
            {
                SceneManager.LoadScene("Intro");
            }
                
        }
    }
}
