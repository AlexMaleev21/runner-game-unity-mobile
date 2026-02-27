using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.UI;

public class SubsystemCheck : MonoBehaviour
{
    IAdaptivePerformance ap;
    public Text Loader;

    void Awake()
    {
        ap = Holder.Instance;
    }

    public void CheckLoader()
    {
        if (ap == null || !ap.Active)
        {
            return;
        }

        var activeLoader = AdaptivePerformanceGeneralSettings.Instance.Manager.activeLoader;
        if (activeLoader != null)
        {
            activeLoader.Initialize();
            Loader.text = "loader initialized";
        }
        else
        {
            Loader.text = "loader is null";
        }
    }
}
