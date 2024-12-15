using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTimeTravel : MonoBehaviour
{
    #region Animation
    [SerializeField] private float timeTravelDuration = 5.0f;
    [SerializeField] private Image uiTimeTravelLine;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Image whiteOutImage;
    private float currentTime = 0;
    private bool isInOtherTime = false;
    private bool isResting = false;
    private Color initLineColor;

    private float animationTime = 1;
    private bool isAnimating = false;
    private float currentAnimatonTime = 0;
    #endregion
    
    
    [SerializeField] private GameObject playerClone;
    [SerializeField] GameObject thisOnePointOnTheStructure;
    Vector3 playerCloneOffset = Vector3.zero;
    private bool teleported = false;
    private bool isSwapped = false;
    Vector3 warpPosition = Vector3.zero;
    void Start()
    {
        initLineColor = uiTimeTravelLine.color;
        currentTime = timeTravelDuration;

        playerCloneOffset = playerClone.transform.position - transform.position;
    }
    
    void Update()
    {
        Debug.Log(Vector3.Distance(playerClone.transform.position, thisOnePointOnTheStructure.transform.position));
        if (!isResting)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (Vector3.Distance(playerClone.transform.position, thisOnePointOnTheStructure.transform.position) < 25.0f)
                {
                    isInOtherTime = true;
                    isAnimating = true;
                }
            }

            if (currentTime > 0 && isInOtherTime)
            {
                currentTime -= Time.deltaTime;
                float scale = currentTime / timeTravelDuration;
                Vector2 lineScale = uiTimeTravelLine.transform.localScale;
                lineScale.x = scale;
                uiTimeTravelLine.transform.localScale = lineScale;

            }
            else if (isInOtherTime)
            {
                isInOtherTime = false;
                currentTime = 0;
                isResting = true;
                Color lineColor = uiTimeTravelLine.color;
                lineColor = new Color(lineColor.r / 2, lineColor.g / 2, lineColor.b / 2, lineColor.a);
                uiTimeTravelLine.color = lineColor;
                isAnimating = true;
            }
        }
        else
        {
            if (currentTime < timeTravelDuration)
            {
                currentTime += Time.deltaTime/2;
                float scale = currentTime / timeTravelDuration;
                Vector2 lineScale = uiTimeTravelLine.transform.localScale;
                lineScale.x = scale;
                uiTimeTravelLine.transform.localScale = lineScale;
            }
            else
            {
                currentTime = timeTravelDuration;
                uiTimeTravelLine.color = initLineColor;
                isResting = false;
            }
        }
        
        if (isAnimating)
        {
            gameObject.GetComponent<CharacterController>().enabled = false;
            if (currentAnimatonTime < animationTime)
            {
                playerCamera.GetComponent<Camera>().fieldOfView = 60 + (-Mathf.Abs(2 * Mathf.Pow(currentAnimatonTime / animationTime, 2) - 1) + 1) * 100;
                Color newColor = whiteOutImage.color;
                newColor.a = (-Mathf.Abs(2 * Mathf.Pow(currentAnimatonTime / animationTime, 2) - 1) + 1) * 1.25f;
                whiteOutImage.color = newColor;
                currentAnimatonTime += Time.deltaTime;
                
                if (whiteOutImage.color.a >= 1.0f && !teleported)
                {
                    if (!isSwapped)
                    {
                        transform.position += playerCloneOffset;
                        isSwapped = true;
                    }
                    else
                    {
                        transform.position -= playerCloneOffset;
                        isSwapped = false;
                    }
                    teleported = true;
                }
            }
            else
            {
                Color newColor = whiteOutImage.color;
                newColor.a = 0;
                whiteOutImage.color = newColor;
                
                currentAnimatonTime = 0;
                isAnimating = false;
                teleported = false;
            }
                
        }
        else
        {
            playerCamera.GetComponent<Camera>().fieldOfView = 60;
            gameObject.GetComponent<CharacterController>().enabled = true;
        }
        
        playerClone.transform.position = transform.position + playerCloneOffset;
    }
    
}

    
