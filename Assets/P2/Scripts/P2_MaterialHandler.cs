using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class P2_MaterialHandler : MonoBehaviour
{
    // Terrorism begins here ----

    public Material DesaturateMaterial;
    public Material DesaturateObjectMaterial;
    public Material DesaturateObjectPlantsMaterial;

    public GameObject LightsHolder;

    public Slider SaturationSlider;
    public Slider LightSaturationSlider;

    public TMP_Dropdown SpaceDropdown;

    int curr_Space; // 0: Image Space, 1: Object Space
    float curr_saturation = 1f;

    List<Light> lights = new();
    List<Color> original_light_colors = new();


    // Start is called before the first frame update
    void Start()
    {
        // Add listener to saturation slider and initialize value
        SaturationSlider.onValueChanged.AddListener((value) => UpdateSliderValues(SaturationSlider, value));
        InitializeSliderValues(SaturationSlider, DesaturateMaterial);

        // Add listeners to the min and max buttons to update the saturation and the slider associated
        SaturationSlider.transform.Find("MinButton").GetComponent<Button>().onClick.AddListener(() => UpdateSliderValues(SaturationSlider, 0f));
        SaturationSlider.transform.Find("MaxButton").GetComponent<Button>().onClick.AddListener(() => UpdateSliderValues(SaturationSlider, 1f));

        // Initialize Space dropdown value
        // (No need to add listener, we're connecting it to UpdateSpace() through the inspector)
        curr_Space = 0; // Image Space
        SpaceDropdown.value = curr_Space;


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

    public void InitializeSliderValues(Slider slider, Material targetMaterial)
    {
        Transform reference = slider.transform.parent;
        slider.value = DesaturateMaterial.GetFloat("_" + reference.name); // name of the parent of SaturationSlider is "Saturation", for example

        UpdateSliderValues(slider, slider.value);
    }


    public void UpdateSliderValues(Slider slider, float value, bool isLight = false)
    {
        slider.value = value;
        if (!isLight) curr_saturation = value;

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
            if (curr_Space == 0) // Image Space
            {
                DesaturateMaterial.SetFloat("_Saturation", curr_saturation);
            }
            else //(curr_Space == 1) // Object Space
            {
                DesaturateObjectMaterial.SetFloat("_Saturation", curr_saturation);
                DesaturateObjectPlantsMaterial.SetFloat("_Saturation", curr_saturation);
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
        curr_Space = idx;
        SpaceDropdown.value = curr_Space;

        if (curr_Space == 0) // Image Space
        {
            // Reset Object Space saturation to 1
            DesaturateObjectMaterial.SetFloat("_Saturation", 1f);
            DesaturateObjectPlantsMaterial.SetFloat("_Saturation", 1f);

            // Set Image Space saturation to the slider's value
            DesaturateMaterial.SetFloat("_Saturation", curr_saturation);
        }
        else //(curr_Space == 1) // Object Space
        {
            // Reset Image Space saturation to 1
            DesaturateMaterial.SetFloat("_Saturation", 1f);

            // Set Object Space saturation to the slider's value
            DesaturateObjectMaterial.SetFloat("_Saturation", curr_saturation);
            DesaturateObjectPlantsMaterial.SetFloat("_Saturation", curr_saturation);
        }
    }
}
