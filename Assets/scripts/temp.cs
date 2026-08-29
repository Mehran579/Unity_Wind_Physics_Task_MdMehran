using UnityEngine;

// Drop this on the same GameObject as your WindZone (alongside CTI_URP_CustomWind).
// It reads the WindZone every frame and pushes calibrated values into the
// Global-scope Shader Graph properties your grass/bush materials read.
[RequireComponent(typeof(WindZone))]
public class temp : MonoBehaviour
{
    [Header("Wind Preset")]
    public WindPreset currentPreset = WindPreset.Custom;

    [System.Serializable]
    public struct WindPresetData
    {
        public float speed;
        public float strength;
        public float directionYaw;
        public float pulseMagnitude;
        public float pulseFrequency;
    }

    public enum WindPreset { Custom, Breezy, Normal, Stormy, Tropical }

    // Preset values - tune these to your scene's scale
    [SerializeField] private WindPresetData breezyPreset = new WindPresetData { speed = 2f, strength = 0.3f, directionYaw = 0f, pulseMagnitude = 0.5f, pulseFrequency = 0.2f };
    [SerializeField] private WindPresetData normalPreset = new WindPresetData { speed = 5f, strength = 0.5f, directionYaw = 0f, pulseMagnitude = 1f, pulseFrequency = 0.5f };
    [SerializeField] private WindPresetData stormyPreset = new WindPresetData { speed = 15f, strength = 1.2f, directionYaw = 0f, pulseMagnitude = 3f, pulseFrequency = 1f };
    [SerializeField] private WindPresetData tropicalPreset = new WindPresetData { speed = 25f, strength = 2f, directionYaw = 0f, pulseMagnitude = 5f, pulseFrequency = 1.5f };

    [Header("Core Controls (0-1 range for Strength)")]
    [Tooltip("Maps to WindZone.windMain - controls overall wind speed")]
    [Range(0f, 30f)] public float windSpeed = 5f;

    [Tooltip("Maps to WindZone.windTurbulence - 0 = calm, 1 = chaotic")]
    [Range(0f, 1f)] public float windStrength = 0.5f;

    [Tooltip("Wind direction in degrees (Y rotation)")]
    [Range(0f, 360f)] public float windDirectionYaw = 0f;

    [Header("WindZone Native Variation (organic gusts)")]
    [Tooltip("How far above/below the main speed gusts swing")]
    [Range(0f, 10f)] public float pulseMagnitude = 1f;

    [Tooltip("How fast gust cycles occur (Hz)")]
    [Range(0f, 3f)] public float pulseFrequency = 0.5f;

    [Header("Derived Shader Values (auto-computed, read-only)")]
    [Tooltip("Derived from windStrength - fine detail chatter")]
    [ReadOnly] public float derivedTurbulence;

    [Tooltip("Derived from pulseMagnitude - sudden burst intensity")]
    [ReadOnly] public float derivedBlast;

    [Tooltip("Derived from pulseFrequency - ripple detail")]
    [ReadOnly] public float derivedRipples;

    [Tooltip("Spatial wavelength - fixed art constant, doesn't track time-based values")]
    [ReadOnly] public float derivedWavelength = 3f;

    [Header("Direction")]
    [Tooltip("Use this GameObject's Y rotation as wind direction (recommended)")]
    public bool useTransformYaw = true;

    // Cached property IDs - avoids re-hashing the string every frame, cheap perf win.
    static readonly int WindSpeedID = Shader.PropertyToID("_WindSpeed");
    static readonly int WindIntensityID = Shader.PropertyToID("_WindIntensity");
    static readonly int WindTurbulenceID = Shader.PropertyToID("_WindTurbulence");
    static readonly int WindWavelengthID = Shader.PropertyToID("_WindWavelength");
    static readonly int WindBlastID = Shader.PropertyToID("_WindBlast");
    static readonly int WindRipplesID = Shader.PropertyToID("_WindRipples");
    static readonly int WindYawID = Shader.PropertyToID("_WindYaw");

    void Reset()
    {
        // Editor-only: auto-populates windZone reference when script is first added
        // Never runs in builds - purely for convenience in the Inspector
        GetComponent<WindZone>();
    }

    void OnValidate()
    {
        // Called when any serialized value changes in Inspector (including preset dropdown)
        // Apply preset values if dropdown changed, otherwise just update derived values
        ApplyPresetIfSelected();
        UpdateDerivedValues();
    }

    void Update()
    {
        WindZone windZone = GetComponent<WindZone>();
        if (windZone == null) return;

        // Push core values into WindZone
        windZone.windMain = windSpeed;
        windZone.windTurbulence = windStrength;
        windZone.windPulseMagnitude = pulseMagnitude;
        windZone.windPulseFrequency = pulseFrequency;

        // Set direction via transform rotation (if using transform yaw)
        if (useTransformYaw)
        {
            windDirectionYaw = transform.eulerAngles.y;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, windDirectionYaw, 0f);
        }

        // Push all values (core + derived) to global shader properties
        Shader.SetGlobalFloat(WindSpeedID, windSpeed);
        Shader.SetGlobalFloat(WindIntensityID, windStrength); // Strength maps to Intensity
        Shader.SetGlobalFloat(WindTurbulenceID, derivedTurbulence);
        Shader.SetGlobalFloat(WindWavelengthID, derivedWavelength);
        Shader.SetGlobalFloat(WindBlastID, derivedBlast);
        Shader.SetGlobalFloat(WindRipplesID, derivedRipples);
        Shader.SetGlobalFloat(WindYawID, windDirectionYaw);
    }

    void ApplyPresetIfSelected()
    {
        // If preset is not Custom, apply its values and reset to Custom if user manually changes sliders
        WindPresetData presetData = GetPresetData(currentPreset);

        if (currentPreset != WindPreset.Custom)
        {
            windSpeed = presetData.speed;
            windStrength = presetData.strength;
            windDirectionYaw = presetData.directionYaw;
            pulseMagnitude = presetData.pulseMagnitude;
            pulseFrequency = presetData.pulseFrequency;
        }
    }

    WindPresetData GetPresetData(WindPreset preset)
    {
        switch (preset)
        {
            case WindPreset.Breezy: return breezyPreset;
            case WindPreset.Normal: return normalPreset;
            case WindPreset.Stormy: return stormyPreset;
            case WindPreset.Tropical: return tropicalPreset;
            default: return new WindPresetData { speed = windSpeed, strength = windStrength, directionYaw = windDirectionYaw, pulseMagnitude = pulseMagnitude, pulseFrequency = pulseFrequency };
        }
    }

    void UpdateDerivedValues()
    {
        // Derive shader values from core inputs
        derivedTurbulence = windStrength;           // Direct semantic match
        derivedBlast = pulseMagnitude;              // Blast = sudden burst above baseline
        derivedRipples = pulseFrequency * 2f;       // Scale frequency to ripple range (tune multiplier as needed)
        derivedWavelength = 3f;                     // Fixed spatial constant - doesn't track time-based values

        // If preset changed to Custom after manual slider adjustment, keep showing current slider values
        // (this is automatic - no extra code needed since we're not forcing values back)
    }

    // Helper to switch preset programmatically (e.g. from UI dropdown)
    public void SetPreset(WindPreset preset)
    {
        currentPreset = preset;
        ApplyPresetIfSelected();
        UpdateDerivedValues();
    }
}

// Custom attribute to make fields read-only in Inspector
public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label)
    {
        GUI.enabled = false;
        UnityEditor.EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif