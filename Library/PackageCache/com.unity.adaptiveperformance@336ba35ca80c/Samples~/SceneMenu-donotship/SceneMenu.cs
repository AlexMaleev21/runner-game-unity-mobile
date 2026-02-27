using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMenu : MonoBehaviour
{
    static int m_AutoTestSceneIndex;
    static bool s_AutoTestSceneRun;

    public UnityEngine.UI.Slider highLevelSlider, midLevelSlider;
    public UnityEngine.UI.Text highLevelSliderValue, midlevelSliderValue;

    public void StartBoostScene()
    {
        SceneManager.LoadScene("Boost");
    }

    public void StartClusterInfoScene()
    {
        SceneManager.LoadScene("ClusterInfo");
    }

    public void StartAdaptiveTransparencyScene()
    {
        SceneManager.LoadScene("AdaptiveTransparency");
    }

    public void StartAdaptiveLUTScene()
    {
        SceneManager.LoadScene("AdaptiveLUT");
    }

    public void StartAdaptiveBatchingScene()
    {
        SceneManager.LoadScene("AdaptiveBatching");
    }

    public void StartAdaptiveSortingScene()
    {
        SceneManager.LoadScene("AdaptiveSorting");
    }

    public void StartAdaptiveResolutionScene()
    {
        SceneManager.LoadScene("AdaptiveResolution");
    }

    public void StartAdaptiveMSAAScene()
    {
        SceneManager.LoadScene("AdaptiveMSAA");
    }

    public void StartAdaptiveShadowScene()
    {
        SceneManager.LoadScene("AdaptiveShadow");
    }

    public void StartAdaptiveLODScene()
    {
        SceneManager.LoadScene("AdaptiveLOD");
    }

    public void StartBottleneckScene()
    {
        SceneManager.LoadScene("Bottleneck");
    }

    public void StartThermalScene()
    {
        SceneManager.LoadScene("Thermal");
    }

    public void StartAPCScene()
    {
        DontDestroyOnLoad(this);
        SceneManager.LoadScene("AutomaticPerformanceControl");
        StartCoroutine(StartupSettingsAPC());
    }

    public void StartAdaptiveFramerateScene()
    {
        SceneManager.LoadScene("AdaptiveFramerate");
    }

    public void StartSettingsScene()
    {
        SceneManager.LoadScene("SettingsCheck");
    }

    public void StartScalerProfilesScene()
    {
        SceneManager.LoadScene("ScalerProfiles");
    }

    public void StartViewDistanceScene()
    {
        SceneManager.LoadScene("AdaptiveViewDistance");
    }

    public void StartPhysicsScene()
    {
        SceneManager.LoadScene("AdaptivePhysics");
    }

    public void StartVisualScriptingScene()
    {
        SceneManager.LoadScene("VisualScripting");
    }

    private void Start()
    {
        ShowSliderValue();
    }
    public void StartSubsystemScene()
    {
        SceneManager.LoadScene("SubsystemCheck");
    }

    IEnumerator StartupSettingsAPC()
    {
        yield return null;
        FindFirstObjectByType<HighLevelLoadManager>().SetLoad(highLevelSlider.value);
        FindFirstObjectByType<MidLevelLoadManager>().SetLoad(midLevelSlider.value);
        Destroy(this.gameObject);
    }

    public void ShowSliderValue()
    {
        highLevelSliderValue.text = highLevelSlider.value.ToString("F2");
        midlevelSliderValue.text = midLevelSlider.value.ToString("F2");
    }

    public void StartVRRScene()
    {
        SceneManager.LoadScene("VariableRefreshRate");
    }

    public void StartCustomScaler()
    {
        SceneManager.LoadScene("CustomScaler");
    }

    public void StartAdaptiveDecalsScaler()
    {
        SceneManager.LoadScene("AdaptiveDecals");
    }

    public void StartAdaptiveLayerCulling()
    {
        SceneManager.LoadScene("AdaptiveLayerCulling");
    }

    public void StartAutoTests()
    {
        DontDestroyOnLoad(this);
        s_AutoTestSceneRun = true;
        NextAutoTestScene();
    }

    public static void NextAutoTestScene()
    {
        Debug.Log(m_AutoTestSceneIndex);
        if (m_AutoTestSceneIndex >= 4)
        {
            KillApplication();
            return;
        }
        SceneManager.LoadScene(++m_AutoTestSceneIndex);
    }

    public static void ExitApplication()
    {
        if (s_AutoTestSceneRun)
        {
            NextAutoTestScene();
            return;
        }
        KillApplication();
    }

    static void KillApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
