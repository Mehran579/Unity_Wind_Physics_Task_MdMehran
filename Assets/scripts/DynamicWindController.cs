//using JetBrains.Annotations;
//using UnityEditor;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static temp;

public class DynamicWindController : MonoBehaviour
{
    public WindZone windzone;

    [Header("shader refrences")]
    static readonly int WindSpeedID = Shader.PropertyToID("Wind_Speed");
    static readonly int WindIntensityID = Shader.PropertyToID("Wind_Intensity");
    static readonly int WindTurbulenceID = Shader.PropertyToID("Wind_Turbulence");         //bush, plants, grass shader graph wind properties cached
    static readonly int WindBlastID = Shader.PropertyToID("Wind_Blast");
    static readonly int WindRipplesID = Shader.PropertyToID("Wind_Ripples");
    static readonly int WindYawID = Shader.PropertyToID("Wind_Yaw");
    public float shaderPulseMagnitudeMultiplier;
    public float shaderPulseFrequencyMultiplier;

    [Header("UI")]
    public Slider speedSlider;
    public Slider strenghtSLider;
    public Slider directionSlider;
    public GameObject panel;
    [Header("Player Inputs")]
    public float speedInput;
    public float strenghtInput;
    public float directionInput;
    public windstates currentstate;
    public enum windstates
    {
        Custom,
        Normal,
        Breezy,
        Windy,
        Tropical,
        Stormy
    }

    [Header("Preset Value")]
    [SerializeField] private WindPresetData breezyPreset = new WindPresetData { speed = 2f, strength = 0.3f, directionYaw = 0f, pulseMagnitude = 0.5f, pulseFrequency = 0.2f };
    [SerializeField] private WindPresetData normalPreset = new WindPresetData { speed = 5f, strength = 0.5f, directionYaw = 0f, pulseMagnitude = 1f, pulseFrequency = 0.5f };
    [SerializeField] private WindPresetData windyPreset = new WindPresetData { speed = 5f, strength = 0.5f, directionYaw = 0f, pulseMagnitude = 1f, pulseFrequency = 0.5f };
    [SerializeField] private WindPresetData tropicalPreset = new WindPresetData { speed = 25f, strength = 2f, directionYaw = 0f, pulseMagnitude = 5f, pulseFrequency = 1.5f };
    [SerializeField] private WindPresetData stormyPreset = new WindPresetData { speed = 15f, strength = 1.2f, directionYaw = 0f, pulseMagnitude = 3f, pulseFrequency = 1f };          //presetting scenarios to start with , the custom still updates live
    bool presetapplied;
    private void Awake()
    {
        currentstate = windstates.Custom;
        speedInput = speedSlider.value;
        strenghtInput = strenghtSLider.value;
        directionInput = directionSlider.value;
        updateUItext(directionInput);

        windzone.windMain = speedInput;
        windzone.windTurbulence = strenghtInput;                                       //synchronizing everything
        windzone.transform.rotation = Quaternion.Euler(0, directionInput, 0);
    }
    void Update()
    {

        Shader.SetGlobalFloat(WindSpeedID, math.remap(0, speedSlider.maxValue, 5, 30, windzone.windMain));
        //Shader.SetGlobalFloat(WindSpeedID, tempspeed);
        //Shader.SetGlobalFloat(WindIntensityID, Mathf.InverseLerp(0.1f,1.1f,windzone.windTurbulence));
        Shader.SetGlobalFloat(WindIntensityID, windzone.windTurbulence);
        Shader.SetGlobalFloat(WindTurbulenceID, 0);
        Shader.SetGlobalFloat(WindBlastID, 0.05f);
        Shader.SetGlobalFloat(WindRipplesID, 0.05f);
        Shader.SetGlobalFloat(WindYawID, 90 + directionInput);
        switch (currentstate)
        {
            case windstates.Custom:
                windzone.windMain = speedInput;
                windzone.windTurbulence = strenghtInput;
                windzone.transform.rotation = Quaternion.Euler(0, directionInput, 0);
                //WindPresetData custompreset = new WindPresetData { speed = speedInput, strength = strenghtInput, directionYaw = directionInput };
                //applypreset(custompreset);
                break;
            case windstates.Normal:
                if (!presetapplied)
                    applypreset(normalPreset);
                break;
            case windstates.Breezy:
                if (!presetapplied)
                    applypreset(breezyPreset);
                break;
            case windstates.Windy:
                if (!presetapplied)
                    applypreset(windyPreset);
                break;
            case windstates.Tropical:
                if (!presetapplied)
                    applypreset(tropicalPreset);
                break;
            case windstates.Stormy:
                if (!presetapplied)
                    applypreset(stormyPreset);
                break;
        }
    }
    void applypreset(WindPresetData presettoapply)
    {
        presetapplied = true;
        windzone.windMain = presettoapply.speed;
        windzone.windTurbulence = presettoapply.strength;
        windzone.transform.rotation = Quaternion.Euler(0, presettoapply.directionYaw, 0);
        speedSlider.value = presettoapply.speed;
        strenghtSLider.value = presettoapply.strength;
        directionSlider.value = presettoapply.directionYaw;
        updateUItext(presettoapply.directionYaw);
        //windzone.windMain = 

    }
    public void OnDropdownChanged(int index)
    {
        presetapplied = false;
        currentstate = (windstates)index;
        bool isCustom = (currentstate == windstates.Custom);
        speedSlider.interactable = isCustom;                           //syncs teh drop down with states
        strenghtSLider.interactable = isCustom;
        directionSlider.interactable = isCustom;
    }
    public TMP_Text speedtext;
    public TMP_Text strengthtext;
    public TMP_Text directiontext;
    public void OnSpeedSliderChanged(float value)
    {
        speedInput = value;
        speedtext.text = value.ToString("f0");
    }
    public void OnStrengthSliderChanged(float value)
    {
        strenghtInput = value;
        strengthtext.text = (value*100).ToString("f0");
    }           
    public void OnDirectionSliderChanged(float value)
    {
        directionInput = value;
        directiontext.text = value.ToString("f0");
    }
    void updateUItext(float diry)
    {
        speedtext.text = windzone.windMain.ToString("f0");
        strengthtext.text = (strenghtSLider.value * 100).ToString("f0");
        directiontext.text = diry.ToString("f0");
    }
}
