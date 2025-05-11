using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class P4_MaterialHandler : MonoBehaviour
{
    // Terrorism begins here ----

    public Material FullscreenMaterial;
    public Material QuantizationMaterialObject;

    public Slider LevelsSlider;

    public GameObject LightsHolder;
    public Slider LightSaturationSlider;

    List<Light> lights = new();
    List<Color> original_light_colors = new();

    public GameObject panel;
    public GameObject panel2;

    public GameObject usingColorsToggle;
    public GameObject colorPicker1;
    public GameObject colorPicker2;

    int curr_space; // 0: Image Space, 1: Object Space
    int curr_colorspace; // 0: Image Space, 1: Object Space

    bool shaderEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        FullscreenMaterial.DisableKeyword("_CHOSENSHADER_CONTOUR");
        FullscreenMaterial.EnableKeyword("_CHOSENSHADER_QUANTIZATION");

        // Initialize Space dropdown value
        // (No need to add listener, we're connecting it to UpdateSpace() through the inspector)
        UpdateSpace(0);

        // Initialize Colorspace dropdown value
        // (No need to add listener, we're connecting it to UpdateSpace() through the inspector)
        UpdateColorspace(0);

        // Add listener to levels slider and initialize value
        LevelsSlider.onValueChanged.AddListener((value) => UpdateSliderValues(LevelsSlider, value));
        UpdateSliderValues(LevelsSlider, 10f); // default value

        ToggleUsingColorGradient(false);

        // LIGHTS ----------------------------------------------------------

        // Get all scene lights and their associated original emission color
        for (int i = 0; i < LightsHolder.transform.childCount; i++)
        {
            Light curr_light = LightsHolder.transform.GetChild(i).GetComponent<Light>();
            lights.Add(curr_light);
            original_light_colors.Add(curr_light.color);
        }

        // Add listener to lights saturation slider and initialize value
        LightSaturationSlider.onValueChanged.AddListener((value) => UpdateSliderValues(LightSaturationSlider, value, true));
        LightSaturationSlider.value = 1f;
        UpdateSliderValues(LightSaturationSlider, 1f, true);

        // Add listeners to the min and max buttons to update the lights saturation and the slider associated
        LightSaturationSlider.transform.Find("MinButton").GetComponent<Button>().onClick.AddListener(() => UpdateSliderValues(LightSaturationSlider, 0f, true));
        LightSaturationSlider.transform.Find("MaxButton").GetComponent<Button>().onClick.AddListener(() => UpdateSliderValues(LightSaturationSlider, 1f, true));
    }

    public void InitializeSliderValues(Slider slider)
    {
        Transform reference = slider.transform.parent;
        if (curr_space == 0) // Image Space
        {
            slider.value = FullscreenMaterial.GetFloat("_" + reference.name); // name of the parent of ThicknessSlider is "Thickness", for example
        }
        else // Object Space
        {
            slider.value = QuantizationMaterialObject.GetFloat("_" + reference.name);
        }

        UpdateSliderValues(slider, slider.value);
    }


    public void UpdateSliderValues(Slider slider, float value, bool isLight = false)
    {
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

        
        if (!isLight)
        {
            Transform reference = slider.transform.parent;
            if (curr_space == 0) // Image Space
            {
                FullscreenMaterial.SetFloat("_" + reference.name, value);
            }
            else //(curr_space == 1) // Object Space
            {
                QuantizationMaterialObject.SetFloat("_" + reference.name, value);
            }
        }
        else
        {
            // For all lights, tweak their saturation
            for (int i = 0; i < lights.Count; i++)
            {
                Color rgb_og_color = original_light_colors[i];
                Vector3 hsv_og_color;
                Color.RGBToHSV(rgb_og_color, out hsv_og_color.x, out hsv_og_color.y, out hsv_og_color.z);
                hsv_og_color.y *= value; // multiply the original saturation by the current one
                lights[i].color = Color.HSVToRGB(hsv_og_color.x, hsv_og_color.y, hsv_og_color.z);
            }
        }
    }


    public void UpdateSpace(int idx)
    {
        curr_space = idx;

        if (curr_space == 0) // Image Space
        {
            // Enable Image Space shader, disable the Object Space one
            if (shaderEnabled) FullscreenMaterial.SetInt("_Enabled", 1);
            QuantizationMaterialObject.SetInt("_Enabled", 0);

            // Set Image Space levels to the previous value
            float levels = QuantizationMaterialObject.GetFloat("_QuantizationLevels");
            FullscreenMaterial.SetFloat("_QuantizationLevels", levels);

            // Reset Object Space levels to 0
            QuantizationMaterialObject.SetFloat("_QuantizationLevels", 0f);

            if (curr_colorspace == 0) // RGB
            {
                FullscreenMaterial.DisableKeyword("_QUANTIZATIONCOLORSPACE_HSV");
                FullscreenMaterial.EnableKeyword("_QUANTIZATIONCOLORSPACE_RGB");
            }
            else //(curr_colorspace == 1) // HSV
            {
                FullscreenMaterial.DisableKeyword("_QUANTIZATIONCOLORSPACE_RGB");
                FullscreenMaterial.EnableKeyword("_QUANTIZATIONCOLORSPACE_HSV");
            }
        }
        else //(curr_space == 1) // Object Space
        {
            // Enable Object Space shader, disable the Image Space one
            if (shaderEnabled) QuantizationMaterialObject.SetInt("_Enabled", 1);
            FullscreenMaterial.SetInt("_Enabled", 0);

            // Set Object Space levels to the previous value
            float levels = FullscreenMaterial.GetFloat("_QuantizationLevels");
            QuantizationMaterialObject.SetFloat("_QuantizationLevels", levels);

            // Reset Image Space levels to 0
            FullscreenMaterial.SetFloat("_QuantizationLevels", 0f);

            if (curr_colorspace == 0) // RGB
            {
                QuantizationMaterialObject.DisableKeyword("_QUANTIZATIONCOLORSPACE_HSV");
                QuantizationMaterialObject.EnableKeyword("_QUANTIZATIONCOLORSPACE_RGB");
            }
            else //(curr_colorspace == 1) // HSV
            {
                QuantizationMaterialObject.DisableKeyword("_QUANTIZATIONCOLORSPACE_RGB");
                QuantizationMaterialObject.EnableKeyword("_QUANTIZATIONCOLORSPACE_HSV");
            }
        }
    }

    public void UpdateColorspace(int idx)
    {
        curr_colorspace = idx;

        if (curr_colorspace == 0) // RGB
        {
            if (curr_space == 0) // Image Space
            {
                FullscreenMaterial.DisableKeyword("_QUANTIZATIONCOLORSPACE_HSV");
                FullscreenMaterial.EnableKeyword("_QUANTIZATIONCOLORSPACE_RGB");
            }
            else //(curr_space == 1) // Object Space
            {
                QuantizationMaterialObject.DisableKeyword("_QUANTIZATIONCOLORSPACE_HSV");
                QuantizationMaterialObject.EnableKeyword("_QUANTIZATIONCOLORSPACE_RGB");
            }

            usingColorsToggle.SetActive(false); // hide
            colorPicker1.SetActive(false); // hide
            colorPicker2.SetActive(false); // hide
            panel2.SetActive(false); // hide
            panel.SetActive(true); // show
        }
        else //(curr_colorspace == 1) // HSV
        {
            if (curr_space == 0) // Image Space
            {
                FullscreenMaterial.DisableKeyword("_QUANTIZATIONCOLORSPACE_RGB");
                FullscreenMaterial.EnableKeyword("_QUANTIZATIONCOLORSPACE_HSV");
            }
            else //(curr_space == 1) // Object Space
            {
                QuantizationMaterialObject.DisableKeyword("_QUANTIZATIONCOLORSPACE_RGB");
                QuantizationMaterialObject.EnableKeyword("_QUANTIZATIONCOLORSPACE_HSV");
            }

            usingColorsToggle.SetActive(true); // show
            colorPicker1.SetActive(true); // show
            colorPicker2.SetActive(true); // show
            panel.SetActive(false); // hide
            panel2.SetActive(true); // show
        }
    }

    public void ToggleEnabled(bool value)
    {
        shaderEnabled = value;
        int enabledInt = shaderEnabled ? 1 : 0;

        if (curr_space == 0) // Image Space
        {
            FullscreenMaterial.SetInt("_Enabled", enabledInt);
        }
        else //(curr_space == 1) // Object Space
        {
            QuantizationMaterialObject.SetInt("_Enabled", enabledInt);
        }
    }

    public void ToggleUsingColorGradient(bool value)
    {
        int usingColorInt = value ? 1 : 0;

        FullscreenMaterial.SetInt("_QuantizationUsingColor", usingColorInt);
        QuantizationMaterialObject.SetInt("_QuantizationUsingColor", usingColorInt);
    }

    public void UpdateColor1(Color col)
    {
        FullscreenMaterial.SetColor("_QuantizationColor1", col);
        QuantizationMaterialObject.SetColor("_QuantizationColor1", col);
    }

    public void UpdateColor2(Color col)
    {
        FullscreenMaterial.SetColor("_QuantizationColor2", col);
        QuantizationMaterialObject.SetColor("_QuantizationColor2", col);
    }
}
