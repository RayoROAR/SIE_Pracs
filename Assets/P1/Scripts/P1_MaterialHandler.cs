using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialHandler : MonoBehaviour
{
    // Terrorism begins here ----

    public Material LitMaterial;
    public Material UnlitMaterial;

    public List<Slider> LitSliders;
    public GameObject LitEmissionPanel;
    private Image litpan;

    public List<Slider> UnlitSliders;
    public GameObject UnlitEmissionPanel;
    private Image unlitpan;

    public TMP_Dropdown ModeDropdown;
    public TMP_Dropdown NormalModeDropdown;

    string prev_Mode_Keyword;
    string prev_NormalMode_Keyword;

    readonly string[] MODES = { "_MODE_COLOR", "_MODE_FUNCTION", "_MODE_NORMAL" };
    readonly string[] NORMALMODES = { "_NORMALMODE_WORLD", "_NORMALMODE_OBJECT", "_NORMALMODE_VIEW" };


    // Start is called before the first frame update
    void Start()
    {
        litpan = LitEmissionPanel.GetComponent<Image>();
        foreach (Slider slider in LitSliders)
        {
            slider.onValueChanged.AddListener((value) => UpdateSliderValues(slider, value, LitMaterial));
            InitializeSliderValues(slider, LitMaterial);
        }

        unlitpan = UnlitEmissionPanel.GetComponent<Image>();
        foreach (Slider slider in UnlitSliders)
        {
            slider.onValueChanged.AddListener((value) => UpdateSliderValues(slider, value, UnlitMaterial));
            InitializeSliderValues(slider, UnlitMaterial);
        }

        foreach (var localKeyword in UnlitMaterial.enabledKeywords)
        {
            if (localKeyword.name.StartsWith("_MODE")) prev_Mode_Keyword = localKeyword.name;
            else if (localKeyword.name.StartsWith("_NORMALMODE")) prev_NormalMode_Keyword = localKeyword.name;

            if (prev_Mode_Keyword != "" && prev_NormalMode_Keyword != "") break;
        }
        ModeDropdown.value = System.Array.IndexOf(MODES, prev_Mode_Keyword);
        NormalModeDropdown.value = System.Array.IndexOf(NORMALMODES, prev_NormalMode_Keyword);
    }


    public void InitializeSliderValues(Slider slider, Material targetMaterial)
    {
        Transform reference = slider.transform.parent;
        if (reference.name == "Red") slider.value = targetMaterial.GetColor("_" + reference.parent.name).r * 255;
        else if (reference.name == "Green") slider.value = targetMaterial.GetColor("_" + reference.parent.name).g * 255;
        else if (reference.name == "Blue") slider.value = targetMaterial.GetColor("_" + reference.parent.name).b * 255;
        else if (reference.name == "Alpha") slider.value = targetMaterial.GetColor("_" + reference.parent.name).a * 255;
        else if (reference.name == "Intensity")
        {
            Color emissionColor = targetMaterial.GetColor("_" + reference.parent.name); // Get HDR color
            slider.value = Mathf.Max(emissionColor.r, emissionColor.g, emissionColor.b); // Extract intensity
        }
        else
        {
            slider.value = LitMaterial.GetFloat("_" + reference.name);
        }

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
        Image targetPanel = targetMaterial == LitMaterial ? litpan : unlitpan;
        if (reference.name == "Red")
        {
            targetPanel.color = new Color(value / 255, targetPanel.color.g, targetPanel.color.b, targetPanel.color.a);

            Color tempColor = targetMaterial.GetColor("_" + reference.parent.name);
            tempColor.r = value / 255;
            targetMaterial.SetColor("_" + reference.parent.name, tempColor);
        }
        else if (reference.name == "Green")
        {
            targetPanel.color = new Color(targetPanel.color.r, value / 255, targetPanel.color.b, targetPanel.color.a);

            Color tempColor = targetMaterial.GetColor("_" + reference.parent.name);
            tempColor.g = value / 255;
            targetMaterial.SetColor("_" + reference.parent.name, tempColor);
        }
        else if (reference.name == "Blue")
        {
            targetPanel.color = new Color(targetPanel.color.r, targetPanel.color.g, value / 255, targetPanel.color.a);

            Color tempColor = targetMaterial.GetColor("_" + reference.parent.name);
            tempColor.b = value / 255;
            targetMaterial.SetColor("_" + reference.parent.name, tempColor);
        }
        else if (reference.name == "Alpha")
        {
            targetPanel.color = new Color(targetPanel.color.r, targetPanel.color.g, targetPanel.color.b, value / 255);

            Color tempColor = targetMaterial.GetColor("_" + reference.parent.name);
            tempColor.a = value / 255;
            targetMaterial.SetColor("_" + reference.parent.name, tempColor);
        }
        //else if (reference.name == "Intensity")
        //{
        //    Color tempColor = targetMaterial.GetColor("_" + reference.parent.name);
        //    targetMaterial.SetColor("_" + reference.parent.name, tempColor);
        //}
        else
        {
            targetMaterial.SetFloat("_" + reference.name, value);
        }

        //Debug.Log(targetPanel.color);
    }


    public void UpdateMode(int idx)
    {
        ModeDropdown.value = idx; // just in case we're updating the dropdown from external sources (random)
        UnlitMaterial.DisableKeyword(prev_Mode_Keyword);
        UnlitMaterial.EnableKeyword(MODES[idx]);
        prev_Mode_Keyword = MODES[idx];
    }


    public void UpdateNormalMode(int idx)
    {
        NormalModeDropdown.value = idx; // just in case we're updating the dropdown from external sources (random)
        UnlitMaterial.DisableKeyword(prev_NormalMode_Keyword);
        UnlitMaterial.EnableKeyword(NORMALMODES[idx]);
        prev_NormalMode_Keyword = NORMALMODES[idx];
    }


    public void RandomizeUnlitValues()
    {
        foreach (Slider slider in UnlitSliders)
        {
            //string parentName = slider.transform.parent.name;
            //bool isRgba = parentName == "Red" || parentName == "Green" || parentName == "Blue" || parentName == "Alpha";
            UpdateSliderValues(slider, Random.Range(0, 256), UnlitMaterial);
            InitializeSliderValues(slider, UnlitMaterial);
        }

        UpdateMode(Random.Range(0, 3));
        UpdateNormalMode(Random.Range(0, 3));
    }


    public void ToggleAutoRandomize(bool isChecked)
    {
        if (isChecked) InvokeRepeating(nameof(RandomizeUnlitValues), 0, 1f);
        else CancelInvoke();
    }
}
