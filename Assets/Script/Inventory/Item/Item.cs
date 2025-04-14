using MFarm.CropPlant;
using Script.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MFarm.Inventory
{
    public class Item : MonoBehaviour //掉落在世界的物品，绑定数据
    {
        public int itemID;
        private SpriteRenderer spriteRenderer; //图片组件
        [FormerlySerializedAs("itemDetails")] public ItemDetails itemInfos; //物品信息
        private BoxCollider2D coll;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            coll = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            Init(itemID);
        }

        public void Init(int ID)
        {
            itemID = ID;
            //Inventory获得当前数据
            itemInfos = InventoryManager.Instance.GetItemDetails(itemID);
            if (itemInfos != null)
            {
                spriteRenderer.sprite =
                    itemInfos.itemOnWorldIcon ? itemInfos.itemOnWorldIcon : itemInfos.itemIcon;
            }

            //修改碰撞体尺寸
            Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
            coll.size = newSize;
            coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            if (itemInfos.itemType == ItemType.ReapableScenery)
            {
                gameObject.AddComponent<ReapItem>();
                gameObject.GetComponent<ReapItem>().InitCropData(itemInfos.itemID);
                gameObject.AddComponent<ItemInterActive>();
            }
        }
    }
}