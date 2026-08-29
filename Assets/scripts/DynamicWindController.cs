using UnityEngine;
using UnityEngine.UI;
using static temp;

public class DynamicWindController : MonoBehaviour
{
    public WindZone windzone;

    static readonly int WindSpeedID = Shader.PropertyToID("Wind_Speed");
    static readonly int WindIntensityID = Shader.PropertyToID("Wind_Intensity");
    static readonly int WindTurbulenceID = Shader.PropertyToID("Wind_Turbulence");         //bush, plants, grass shader graph wind properties cached
    static readonly int WindBlastID = Shader.PropertyToID("Wind_Blast");
    static readonly int WindRipplesID = Shader.PropertyToID("Wind_Ripples");
    static readonly int WindYawID = Shader.PropertyToID("Wind_Yaw");


    [Header("UI")]
    public Slider speedSlider;
    public Slider strenghtSLider;
    public Slider directionSlider;

    [Header("Player Inputs")]
    public float speedInput;
    public float strenghtInput;
    public float directionInput;
    public windStates currentstate;
    public enum windStates
    {
        Custom,
        Normal,
        Breezy,
        Tropical,
        Stormy
    }

    [Header("Preset Value")]
    [SerializeField] private WindPresetData breezyPreset = new WindPresetData { speed = 2f, strength = 0.3f, directionYaw = 0f, pulseMagnitude = 0.5f, pulseFrequency = 0.2f };
    [SerializeField] private WindPresetData normalPreset = new WindPresetData { speed = 5f, strength = 0.5f, directionYaw = 0f, pulseMagnitude = 1f, pulseFrequency = 0.5f };
    [SerializeField] private WindPresetData stormyPreset = new WindPresetData { speed = 15f, strength = 1.2f, directionYaw = 0f, pulseMagnitude = 3f, pulseFrequency = 1f };
    [SerializeField] private WindPresetData tropicalPreset = new WindPresetData { speed = 25f, strength = 2f, directionYaw = 0f, pulseMagnitude = 5f, pulseFrequency = 1.5f };
    void Update()
    {
        switch (currentstate)
        {
            case windStates.Custom:
                windzone.windMain = speedInput;
                windzone.windTurbulence = strenghtInput;
                windzone.transform.rotation = Quaternion.Euler(0, directionInput, 0);
                //WindPresetData custompreset = new WindPresetData { speed = speedInput, strength = strenghtInput, directionYaw = directionInput };
                //applypreset(custompreset);
                break;
            case windStates.Normal:
                applypreset(normalPreset);
                break;
            case windStates.Breezy:
                applypreset(breezyPreset);
                break;
            case windStates.Tropical:
                applypreset(tropicalPreset);

                break;
            case windStates.Stormy:
                applypreset(stormyPreset);

                break;
        }
    }
    void applypreset(WindPresetData presettoapply)
    {
        windzone.windMain = presettoapply.speed;
        windzone.windTurbulence = presettoapply.strength;
        windzone.transform.rotation = Quaternion.Euler(0, presettoapply.directionYaw, 0); 
        speedSlider.value = presettoapply.speed;
        strenghtSLider.value = presettoapply.strength;
        directionSlider.value = presettoapply.directionYaw;
        //windzone.windMain = 
    }
    public void OnDropdownChanged(int index)
    {
        currentstate = (windStates)index;
        bool isCustom = (currentstate == windStates.Custom);
        speedSlider.interactable = isCustom;                           //syncs teh drop down with states
        strenghtSLider.interactable = isCustom;
        directionSlider.interactable = isCustom;
    }
    public void OnSpeedSliderChanged(float value) => speedInput = value;
    public void OnStrengthSliderChanged(float value) => strenghtInput = value;              //runs when slider is changed;
    public void OnDirectionSliderChanged(float value) => directionInput = value;
}
