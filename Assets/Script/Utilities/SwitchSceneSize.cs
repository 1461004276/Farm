using Cinemachine;
using UnityEngine;

namespace Script.Utilities
{
    public class SwitchSceneSize : MonoBehaviour //控制切换边界时的类，每张地图边界不同，每次切换地图时调用此类，更新一下边界让Cinamachine知道
    {
        private void OnEnable()
        {
            EventSystem.AfterSceneLoadedEvent += SwitchConfinerSize;//加载注册事件
        }
        private void OnDisable()
        {
            EventSystem.AfterSceneLoadedEvent -= SwitchConfinerSize;
        }
        private void SwitchConfinerSize()
        {
            PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();
            CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();
            confiner.m_BoundingShape2D = confinerShape;
            //每当切换场景的时候调用此函数清理一下路劲缓存，清除之前的边界信息
            confiner.InvalidatePathCache();
        }
    }
}