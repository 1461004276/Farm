using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class LightControl : MonoBehaviour
{
    public LightPattenList_SO lightData;
    private Light2D currentLight;
    private LightDetails currentLightDetails;
    private void Awake()
    {
        currentLight = GetComponent<Light2D>();
    }
    //实际切换灯光
    public void ChangeLightShift(Season season,LightShift lightShift,float timeDifference)
    {
        currentLightDetails = lightData.GetLightDetails(season, lightShift);
        if (timeDifference < Prams.lightChangeDuration)
        {
            //计算颜色差值
            var colorOffst = (currentLightDetails.lightColor - currentLight.color) / Prams.lightChangeDuration * timeDifference;
            currentLight.color += colorOffst;
            DOTween.To(() => currentLight.color, c => currentLight.color = c, currentLightDetails.lightColor, Prams.lightChangeDuration - timeDifference);
            DOTween.To(() => currentLight.intensity, i => currentLight.intensity = i, currentLightDetails.lightAmount, Prams.lightChangeDuration - timeDifference);
        }
        if (timeDifference >= Prams.lightChangeDuration)
        {
            currentLight.color = currentLightDetails.lightColor;
            currentLight.intensity = currentLightDetails.lightAmount;
        }
    }
}