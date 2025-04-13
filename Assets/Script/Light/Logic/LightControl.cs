using DG.Tweening;
using Script.Utilities;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightControl : MonoBehaviour
{
    public LightPattenList_SO lightData;
    private Light2D _currentLight;
    private LightDetails _currentLightDetails;

    private void Awake()
    {
        _currentLight = GetComponent<Light2D>();
    }

    //实际切换灯光
    public void ChangeLightShift(Season season, LightShift lightShift, float timeDifference)
    {
        _currentLightDetails = lightData.GetLightDetails(season, lightShift);
        if (timeDifference < Prams.lightChangeDuration)
        {
            //计算颜色差值
            var colorOffst = (_currentLightDetails.lightColor - _currentLight.color) / Prams.lightChangeDuration *
                             timeDifference;
            _currentLight.color += colorOffst;
            DOTween.To(() => _currentLight.color,
                c => _currentLight.color = c, _currentLightDetails.lightColor,
                Prams.lightChangeDuration - timeDifference);
            DOTween.To(() => _currentLight.intensity,
                i => _currentLight.intensity = i,
                _currentLightDetails.lightAmount, Prams.lightChangeDuration - timeDifference);
        }

        if (timeDifference >= Prams.lightChangeDuration)
        {
            _currentLight.color = _currentLightDetails.lightColor;
            _currentLight.intensity = _currentLightDetails.lightAmount;
        }
    }
}