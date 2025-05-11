using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ColorPicker : MonoBehaviour
{
    public GameObject buttonPreviewObj;
    public GameObject colorPickerObj;
    bool pickerVisible = false;
    FlexibleColorPicker fcp;

    Image buttonPreviewColorImg;
    Image buttonPreviewAlphaImg;

    public ColorUpdateEvent onColorChange;

    [Serializable]
    public class ColorUpdateEvent : UnityEvent<Color> { }

    // Start is called before the first frame update
    void Start()
    {
        colorPickerObj.SetActive(false); // hide

        buttonPreviewColorImg = buttonPreviewObj.transform.Find("ColorPreview/OpaquePreview").GetComponent<Image>(); // get image rect
        buttonPreviewAlphaImg = buttonPreviewObj.transform.Find("ColorPreview/PreviewAlphaBackground/AlphaPreview").GetComponent<Image>(); // get image rect

        fcp = colorPickerObj.GetComponent<FlexibleColorPicker>();
        UpdateButtonPreview(fcp.color);
    }

    public void TogglePickerVisibility()
    {
        pickerVisible = !pickerVisible;
        colorPickerObj.SetActive(pickerVisible); // show/hide
        UpdateButtonPreview(fcp.color);
    }

    public void UpdateButtonPreview(Color col)
    {
        onColorChange.Invoke(col); // send the color signal up to the Material Handler
        if (buttonPreviewColorImg && buttonPreviewAlphaImg)
        {
            buttonPreviewAlphaImg.color = col;

            Color fullAlphaCol = col;
            fullAlphaCol.a = 1f;
            buttonPreviewColorImg.color = fullAlphaCol;
        }
    }
}
