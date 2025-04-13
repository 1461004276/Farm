using Script.Utilities;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightControl[] sceneLights;
    private LightShift currentLightShift;
    private Season currentSeaon;
    private float timeDifference = Prams.lightChangeDuration;

    private void OnEnable()
    {
        //提示:委托函数事件是一个装函数方法的变量,但是并不是每个委托只能装一个函数变量,委托可以装很多不同的函数然后让它们同时执行!
        EventSystem.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventSystem.LightShiftChangeEvent += OnLightShiftChangeEvent;
        EventSystem.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventSystem.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventSystem.LightShiftChangeEvent -= OnLightShiftChangeEvent;
        EventSystem.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void OnStartNewGameEvent(int index)
    {
        currentLightShift = LightShift.Morning;
    }

    private void OnLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        currentSeaon = season;
        this.timeDifference = timeDifference;
        if (currentLightShift != lightShift)
        {
            currentLightShift = lightShift;
            foreach (LightControl light in sceneLights)
            {
                //lightcontrol 改变灯光的方法
                light.ChangeLightShift(currentSeaon, currentLightShift, timeDifference);
            }
        }
    }

    private void OnAfterSceneLoadedEvent()
    {
        sceneLights = FindObjectsOfType<LightControl>();
        foreach (LightControl light in sceneLights)
        {
            //lightControl 改变灯光的方法
            light.ChangeLightShift(currentSeaon, currentLightShift, timeDifference);
        }
    }
}