using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.UI;

public class SettingsCheck : MonoBehaviour
{
    IAdaptivePerformance ap;
    GameObject apm;
    public Text AdaptivePerformance, AdaptivePerformanceEnabled, AutoControlMode, Indexer, ThermalActionDelay, PerformanceActionDelay, ThermalStateMode, ThermalSafeRange, Logging, LoggingFrequency, PerformanceMode;
    // Start is called before the first frame update
    void Start()
    {
        apm = GameObject.Find("AdaptivePerformanceManager");

        ap = Holder.Instance;

        if (ap == null || !ap.Active)
        {
            return;
        }
        ParseBool(ap.Active, AdaptivePerformance);
        ParseBool(apm.activeSelf, AdaptivePerformanceEnabled);
        IAdaptivePerformanceSettings settings = AdaptivePerformanceGeneralSettings.Instance.Manager.activeLoader.GetSettings();
        if (settings == null)
            return;
        ParseBool(ap.DevicePerformanceControl.AutomaticPerformanceControl, AutoControlMode);
        ParseBool(ap.DevelopmentSettings.Logging, Logging);
        LoggingFrequency.text += ap.DevelopmentSettings.LoggingFrequencyInFrames;
        ParseBool(settings.indexerSettings.active, Indexer);
        if (!settings.indexerSettings.active)
        {
            return;
        }
        PerformanceMode.text += ap.PerformanceStatus.PerformanceMode;

        ThermalActionDelay.text += settings.indexerSettings.thermalActionDelay;
        PerformanceActionDelay.text += settings.indexerSettings.performanceActionDelay;
    }

    void ParseBool(bool check, Text output)
    {
        if (check)
            output.text += " ACTIVE";
        else output.text += " NOT ACTIVE";
    }

    public void ToggleAP()
    {
        if (apm == null)
            return;

        apm.SetActive(!apm.activeSelf);
        ParseBool(apm.activeSelf, AdaptivePerformanceEnabled);
    }
}
