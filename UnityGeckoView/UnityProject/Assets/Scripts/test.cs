using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public Image Image;
    private RectTransform _rawImageRect;

    public int Width;
    public int Height;

    private void Awake()
    {

        _rawImageRect = Image.GetComponent<RectTransform>();

    }

    public void pointerDown(BaseEventData data) {

        PointerEventData eventData = (PointerEventData)data;

        Vector3 positionInRect = PointerPositionInRect(eventData.position);

        Debug.Log(positionInRect);
    }

    public Vector2 PointerPositionInRect(Vector3 screenPoint)
    {

        Camera thisCamera = Camera.main;
        Debug.Assert(thisCamera.name == "CenterEyeAnchor");
        Vector2 positionInRect = new Vector2();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rawImageRect,
            screenPoint, thisCamera, out positionInRect);
        // Debug.Log("main camera is: " + Camera.main.name);

        // Take care of the pivots and their effect on position
        positionInRect.x += _rawImageRect.pivot.x * _rawImageRect.rect.width;
        positionInRect.y += (_rawImageRect.pivot.y * _rawImageRect.rect.height);

        Debug.Assert(Math.Abs(_rawImageRect.pivot.y) > 0);
        // Change coordinate system 
        positionInRect.y += -_rawImageRect.rect.height;
        positionInRect.y = Math.Abs(positionInRect.y);
        // Debug.Log(positionInRect);

        // get the screen dimensions and divide them by the rectangle's screen dimensions for scaling
        float screenWidth = Width; //rect.width;
        float screenHeight = Height; //rect.height;

        float xScale = screenWidth / _rawImageRect.rect.width; // rectWidthInScreen;
        float yScale = screenHeight / _rawImageRect.rect.height; // rectHeightInScreen;

        Vector2 positionInWebView = new Vector2(positionInRect.x * xScale, positionInRect.y * yScale);

        return positionInWebView;
    }
}
