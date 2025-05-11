using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class P3_MaterialHandler : MonoBehaviour
{
    // Terrorism begins here ----

    public Material FullscreenMaterial;
    public Material ContourMaterialObject;

    public Slider ThicknessSlider;
    public Slider NormalThresholdSlider;
    public Slider ColorThresholdSlider;

    public GameObject panel;
    public GameObject panel2;

    float thickness = 0.6f;
    float normal_threshold = 1.34f;
    float color_threshold = 0.89f;

    int curr_space; // 0: Image Space, 1: Object Space


    // Start is called before the first frame update
    void Start()
    {
        FullscreenMaterial.DisableKeyword("_CHOSENSHADER_QUANTIZATION");
        FullscreenMaterial.EnableKeyword("_CHOSENSHADER_CONTOUR");
        FullscreenMaterial.SetInt("_Enabled", 1);

        FullscreenMaterial.SetFloat("_Thickness", thickness);
        FullscreenMaterial.SetFloat("_NormalThreshold", normal_threshold);
        FullscreenMaterial.SetFloat("_ColorThreshold", color_threshold);
        ContourMaterialObject.SetFloat("_Thickness", thickness);

        // Add listener to thickness slider and initialize value
        ThicknessSlider.onValueChanged.AddListener((value) => UpdateSliderValues(ThicknessSlider, value, ref thickness));
        InitializeSliderValues(ThicknessSlider, ref thickness);

        // Add listener to normal threshold slider and initialize value
        NormalThresholdSlider.onValueChanged.AddListener((value) => UpdateSliderValues(NormalThresholdSlider, value, ref normal_threshold));
        InitializeSliderValues(NormalThresholdSlider, ref normal_threshold);

        // Add listener to color threshold slider and initialize value
        ColorThresholdSlider.onValueChanged.AddListener((value) => UpdateSliderValues(ColorThresholdSlider, value, ref color_threshold));
        InitializeSliderValues(ColorThresholdSlider, ref color_threshold);

        // Initialize Space dropdown value
        // (No need to add listener, we're connecting it to UpdateSpace() through the inspector)
        UpdateSpace(0);
    }

    public void InitializeSliderValues(Slider slider, ref float variableUpdated)
    {
        Transform reference = slider.transform.parent;
        if (curr_space == 0 || slider == NormalThresholdSlider || slider == ColorThresholdSlider) // Image Space
        {
            slider.value = FullscreenMaterial.GetFloat("_" + reference.name); // name of the parent of ThicknessSlider is "Thickness", for example
        }
        else // Object Space
        {
            slider.value = ContourMaterialObject.GetFloat("_" + reference.name);
        }

        UpdateSliderValues(slider, slider.value, ref variableUpdated);
    }


    public void UpdateSliderValues(Slider slider, float value, ref float variableUpdated)
    {
        variableUpdated = value;
        slider.value = value; // just in case we're updating the color from external sources (random)
        // Find the corresponding TextMeshPro label inside the same parent
        TMP_Text valueLabel = slider.transform.parent.Find("Value").GetComponent<TMP_Text>();

        if (value % 1 == 0) // whole number
        {
            valueLabel.text = value.ToString();
        }
        else
        {
            valueLabel.text = value.ToString("F2"); // round to 2 decimal places
        }

        Transform reference = slider.transform.parent;
        if (curr_space == 0) // Image Space
        {
            FullscreenMaterial.SetFloat("_" + reference.name, value);
        }
        else //(curr_space == 1) // Object Space
        {
            ContourMaterialObject.SetFloat("_" + reference.name, value);
        }
    }


    public void UpdateSpace(int idx)
    {
        curr_space = idx;

        if (curr_space == 0) // Image Space
        {
            // Reset Object Space thickness to 0
            ContourMaterialObject.SetFloat("_Thickness", 0f);

            // Set Image Space thickness to the slider's value
            FullscreenMaterial.SetFloat("_Thickness", thickness);

            NormalThresholdSlider.transform.parent.gameObject.SetActive(true); // show
            ColorThresholdSlider.transform.parent.gameObject.SetActive(true); // show
            panel2.SetActive(false); // hide
            panel.SetActive(true); // show
        }
        else //(curr_space == 1) // Object Space
        {
            // Reset Image Space thickness to 0
            FullscreenMaterial.SetFloat("_Thickness", 0f);

            // Set Object Space thickness to the slider's value
            ContourMaterialObject.SetFloat("_Thickness", thickness);

            NormalThresholdSlider.transform.parent.gameObject.SetActive(false); // hide
            ColorThresholdSlider.transform.parent.gameObject.SetActive(false); // hide
            panel.SetActive(false); // hide
            panel2.SetActive(true); // show
        }
    }

    public void UpdateOutlineColor(Color col)
    {
        FullscreenMaterial.SetColor("_Color", col);
        ContourMaterialObject.SetColor("_Color", col);
    }
}
