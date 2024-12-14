using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class ResizeVideo : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Image blackShadeInImage;
    [SerializeField] private Image blackShadeOutImage;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private float shadeTime;

    private float timer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height + 50);
    }

    // Update is called once per frame
    void Update()
    {
        if (blackShadeInImage.color.a > 0)
        {
            Color newColor = blackShadeInImage.color;
            newColor.a -= Time.deltaTime * shadeTime;
            blackShadeInImage.color = newColor;
        }
        
        
        if (timer > 53)
        {
            if (blackShadeOutImage.color.a < 1)
            {
                Color newColor = blackShadeOutImage.color;
                newColor.a += Time.deltaTime * shadeTime;
                blackShadeOutImage.color = newColor;
            }
            else
            {
                SceneManager.LoadScene("Game");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene("Game");

        timer += Time.deltaTime;
    }
}
