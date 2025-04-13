using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject[] panels;
    public void SwitchPanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == index)
            {
                //Unity在Hierarchy同层级面板中渲染顺序是按照从上到下顺序的，所以这里需要将当前面板的transform移动到列表最下面(最后)
                //SetAsLastSibling()函数:将当前object中的Hierarchy面板的transfrom移动到列表最下面(最后)
                panels[i].transform.SetAsLastSibling();
            }
        }
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("EXIT GAME");
    }
    //一看到Sibling就应该想到和Hierarchy有关
}