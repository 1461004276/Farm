using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;
using System.Linq;
using Script.Utilities;

public class ItemEditor : EditorWindow
{
    private ItemDataList_SO dataBase; //���������ݿ��е�����
    private List<ItemDetails> itemList = new List<ItemDetails>();//newһ��ItemDtails,�൱�����ݿ��е���ϸ�����б�
    private VisualTreeAsset itemRowTemplate; //��ȡ�Լ���UIToolkit�ж������ʽ
    private ListView itemListView;//���VisualElement�еĹ����б�
    private ScrollView itemDetailsSection;//��ȡUITookit�Ҳ����
    private ItemDetails activeItem;//��ȡ��ǰѡ�еĵ���(��)
    private VisualElement iconPreview;//iconԤ��
    private Sprite defaultIcon;//Ĭ��Ԥ��ͼƬ

    [MenuItem("Doggo/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
/*        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);*/

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Bulider/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        //ֱ��ͨ������·����ʽ�õ���ʽģ��
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Bulider/ItemRowTemplate.uxml");

        //�õ�Ĭ��ͼƬ
        defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_M.png");

        //������ֵ
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");//����ҳ�渳ֵ
        iconPreview = itemDetailsSection.Q<VisualElement>("Icon");//�ҵ���ǰ��������ҳ���Icon
        root.Q<Button>("AddButton").clicked += OnAddItemClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteClicked;

        //��������
        LoadDataBase();

        //����ListView
        GenerateListView();
    }

    #region �����¼�
    private void OnDeleteClicked()
    {
        itemList.Remove(activeItem);
        itemListView.Rebuild();//ˢ��һ�¹����б���ˢ������
        itemDetailsSection.visible = false;//ɾ���б������ݶ�ʧ�����Թر��������
    }

    private void OnAddItemClicked()
    {
        ItemDetails newItem = new ItemDetails();
        newItem.itemID = 1001 + itemList.Count;
        newItem.itemName = "NEW ITEM";
        itemList.Add(newItem);
        itemListView.Rebuild();//ˢ��һ������
    }
    #endregion
    private void LoadDataBase()//�õ�Assets�����Ǵ��������ݿ��ļ�
    {
        var dataArray = AssetDatabase.FindAssets("ItemDataList_SO");//������Assets���ҵ�����ΪItemDataList_SO����Դ�ļ�����Ϊ����,����ø����ļ���GUID
        if (dataArray.Length > 1)//���Assets�д���ItemDataList_SO����Դ�ļ�
        {
            var path = AssetDatabase.GUIDToAssetPath(dataArray[0]);//ͨ����õ������еľ���һ���ļ���GUID�������ļ���·��
            //��Ϊ��ֻ��һ��ItemDataList_SO�ļ�,�����ҿ���ֱ��дdataArray[0]
            dataBase = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList_SO)) as ItemDataList_SO;//��ͨ���ļ�·���������ݿ���Դ
        }
        itemList = dataBase.itemDetailsList;//Ϊ�б���ֵ
        EditorUtility.SetDirty(dataBase);//��עһ������,�����עһ�����ݷ����޷���������
    }
    private void GenerateListView()//����ListView��������
    {
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree(); //ÿ����һ������ListView�����Ϳ�¡һ����ʽģ��
        Action<VisualElement, int> bindItem = (e, i) =>
         {
             if (i < itemList.Count)//����ȷ�����С���б��е�������������ᱨ��
             {
                 if (itemList[i].itemIcon != null)
                 {
                     e.Q<VisualElement>("Icon").style.backgroundImage = itemList[i].itemIcon.texture;//�����ݿ��е����ݸ�ֵ�󶨸�UI Toolkit
                 }
                 e.Q<Label>("Name").text = itemList[i] == null ? "NO ITEM" : itemList[i].itemName;
             }
         };
        //e.Q<VisualElement>:���Ǵ���UI Toolkit�о����Ԫ��,VisualElement��������Ŀհ�����
        //e.Q<Label>���������ı���ǩ���ڴ��ô��뽫����ʵ����

        itemListView.fixedItemHeight = 60;//�̶����ָ߶�
        itemListView.itemsSource = itemList;//���б��е����ݷ��������
        itemListView.makeItem = makeItem;//�Ƚ�ģ���¡�ڹ����У����ǹ��־�ӵ����Icon��Name����ToolkitԪ��
        itemListView.bindItem = bindItem;//���������Actionί�о���ʵ�ֽ����ݿ�ָ�����ݰ���Toolkit

        itemListView.onSelectionChange += OnListSelectionChange;//�¼�����ί��
        itemDetailsSection.visible = false;//�Ҳ������Ϣ���ɼ�(Ĭ�ϵ��ȥ��ʱ�򲻻���ʾ�κ���Ʒ��Ϣ��ֻ�е��һ���������������ʾ)
    }
    private void OnListSelectionChange(IEnumerable<object> selectedItem)
    {
        activeItem = (ItemDetails)selectedItem.First();
        GetItemDetails();
        itemDetailsSection.visible = true;
    }
    private void GetItemDetails()
    {
        itemDetailsSection.MarkDirtyRepaint();//����һ֡�������ػ棬���Գ��������ع���
        itemDetailsSection.Q<IntegerField>("ItemID").value = activeItem.itemID;//����ǰѡ�еĵ������IDֵ���ݸ����������б��е�ID
        itemDetailsSection.Q<IntegerField>("ItemID").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemID = evt.newValue;
        });//��IDֵ�����ı�ִ�лص�����

        itemDetailsSection.Q<TextField>("ItemName").value = activeItem.itemName;
        itemDetailsSection.Q<TextField>("ItemName").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemName = evt.newValue;
            itemListView.Rebuild();//�ع�,ˢ��һ��,��������������޸��˵��ߵ�����,�������б��е����־ͻ�ˢ���ع�
        });

        iconPreview.style.backgroundImage = activeItem.itemIcon == null?defaultIcon.texture:activeItem.itemIcon.texture;//��ȡ��ǰѡ�еĵ��ߵ�Icon,���Ϊnull�ͷ���Ĭ��icon
        itemDetailsSection.Q<ObjectField>("ItemIcon").value = activeItem.itemIcon;
        itemDetailsSection.Q<ObjectField>("ItemIcon").RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = evt.newValue as Sprite;
            activeItem.itemIcon = newIcon;

            iconPreview.style.backgroundImage = newIcon == null ? defaultIcon.texture : newIcon.texture;
            itemListView.Rebuild();
        });

        //�������б����İ�
        itemDetailsSection.Q<ObjectField>("ItemSprite").value = activeItem.itemOnWorldIcon;
        itemDetailsSection.Q<ObjectField>("ItemSprite").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemOnWorldIcon = (Sprite)evt.newValue;
        });

        itemDetailsSection.Q<EnumField>("ItemType").Init(activeItem.itemType);
        itemDetailsSection.Q<EnumField>("ItemType").value = activeItem.itemType;
        itemDetailsSection.Q<EnumField>("ItemType").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemType = (ItemType)evt.newValue;
        });

        itemDetailsSection.Q<TextField>("Description").value = activeItem.itemDescription;
        itemDetailsSection.Q<TextField>("Description").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemDescription = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("ItemUseRadius").value = activeItem.itemUseRadius;
        itemDetailsSection.Q<IntegerField>("ItemUseRadius").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemUseRadius = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanPickedup").value = activeItem.canPickedUp;
        itemDetailsSection.Q<Toggle>("CanPickedup").RegisterValueChangedCallback(evt =>
        {
            activeItem.canPickedUp = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanDropped").value = activeItem.canDropped;
        itemDetailsSection.Q<Toggle>("CanDropped").RegisterValueChangedCallback(evt =>
        {
            activeItem.canDropped = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanCarried").value = activeItem.canCarried;
        itemDetailsSection.Q<Toggle>("CanCarried").RegisterValueChangedCallback(evt =>
        {
            activeItem.canCarried = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("Price").value = activeItem.itemPrice;
        itemDetailsSection.Q<IntegerField>("Price").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemPrice = evt.newValue;
        });

        itemDetailsSection.Q<Slider>("SellPercentage").value = activeItem.sellPercentage;
        itemDetailsSection.Q<Slider>("SellPercentage").RegisterValueChangedCallback(evt =>
        {
            activeItem.sellPercentage = evt.newValue;
        });
    }
}