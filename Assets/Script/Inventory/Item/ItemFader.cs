using DG.Tweening;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
//挂载在需要透明化的物体身上，用于物品逐渐显示或隐藏
public class ItemFader : MonoBehaviour 
{
    private SpriteRenderer spriteRender;

    private void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
    }
    /// <summary>
    /// 逐渐恢复颜色
    /// </summary>
    public void Fadein()
    {
        Color targetColor = new Color(1, 1, 1, 1);
        spriteRender.DOColor(targetColor, Prams.ItemfadeDuration);
    }
    /// <summary>
    /// 逐渐半透明
    /// </summary>
    public void FadeOut()
    {
        Color targetColor = new Color(1, 1, 1, Prams.targetAlpha);
        spriteRender.DOColor(targetColor, Prams.ItemfadeDuration);
    }
}