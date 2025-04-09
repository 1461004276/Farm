using UnityEngine;

public class TriggleItemFader : MonoBehaviour 
{
    //当玩家进入道具范围时触发物体身上Fader函数类时物体透明化，挂载在Player身上
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemFader[] fader = collision.GetComponentsInChildren<ItemFader>();
        if (fader.Length > 0)
        {
            foreach (var obj in fader)
            {
                obj.FadeOut();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        ItemFader[] fader = collision.GetComponentsInChildren<ItemFader>();
        if (fader.Length > 0)
        {
            foreach (var obj in fader)
            {
                obj.Fadein();
            }
        }
    }
}