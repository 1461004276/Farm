using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Utilities
{
    public class BushesSettings : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer bushesRenderer;
        [FormerlySerializedAs("_bushesSprites")] [SerializeField] private List<Sprite> bushesSprites;
        public BushesType bushesType;

        private void Start()
        {
            if (bushesType == BushesType.None)
            {
                bushesRenderer.sprite = bushesSprites[Random.Range(0,bushesSprites.Count)];
            }
            else
            {
                bushesRenderer.sprite = bushesSprites[(int)bushesType];
            }
        }

        [ContextMenu("Update bushes sprites")]
        public void UpdateBushesSprites()
        {
            bushesRenderer.sprite = bushesSprites[(int)bushesType];
        }
    }
}
