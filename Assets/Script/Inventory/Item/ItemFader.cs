using DG.Tweening;
using Script.Utilities;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
//挂载在需要透明化的物体身上，用于物品逐渐显示或隐藏
public class ItemFader : MonoBehaviour 
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    /// <summary>
    /// 逐渐恢复颜色
    /// </summary>
    public void Fadein()
    {
        Color targetColor = new Color(1, 1, 1, 1);
        _spriteRenderer.DOColor(targetColor, Prams.ItemfadeDuration);
    }
    /// <summary>
    /// 逐渐半透明
    /// </summary>
    public void FadeOut()
    {
        Color targetColor = new Color(1, 1, 1, Prams.targetAlpha);
        _spriteRenderer.DOColor(targetColor, Prams.ItemfadeDuration);
    }
}