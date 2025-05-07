using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class P3_MaterialHandler : MonoBehaviour
{
    // Terrorism begins here ----

    public Material ContourMaterial;

    public Slider ThicknessSlider;

    public TMP_Dropdown SpaceDropdown;

    string prev_Space_Keyword;

    readonly string[] SPACES = { "_SPACE_IMAGESPACE", "_SPACE_OBJECTSPACE"};


    // Start is called before the first frame update
    void Start()
    {
        // Add listener to saturation slider and initialize value
        ThicknessSlider.onValueChanged.AddListener((value) => UpdateSliderValues(ThicknessSlider, value, ContourMaterial));
        InitializeSliderValues(ThicknessSlider, ContourMaterial);

        // Initialize Space dropdown value depending on the material's value
        // (No need to add listener, we're connecting it to UpdateSpace() through the inspector)
        foreach (var localKeyword in ContourMaterial.enabledKeywords)
        {
            if (localKeyword.name.StartsWith("_SPACE")) prev_Space_Keyword = localKeyword.name;

            if (prev_Space_Keyword != "") break; // This is more useful when having multiple keywords but I'll still keep it for consistency sake
        }
        SpaceDropdown.value = System.Array.IndexOf(SPACES, prev_Space_Keyword);
    }

    public void InitializeSliderValues(Slider slider, Material targetMaterial)
    {
        Transform reference = slider.transform.parent;
        slider.value = ContourMaterial.GetFloat("_" + reference.name); // name of the parent of ThicknessSlider is "Thickness", for example

        UpdateSliderValues(slider, slider.value, targetMaterial);
    }


    public void UpdateSliderValues(Slider slider, float value, Material targetMaterial)
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

        Transform reference = slider.transform.parent;
        targetMaterial.SetFloat("_" + reference.name, value);
    }


    public void UpdateSpace(int idx)
    {
        SpaceDropdown.value = idx; // just in case we're updating the dropdown from external sources (random)
        ContourMaterial.DisableKeyword(prev_Space_Keyword);
        ContourMaterial.EnableKeyword(SPACES[idx]);
        prev_Space_Keyword = SPACES[idx];
    }
}
