using Script.Utilities;
using UnityEngine;

namespace MFarm.Transition
{
    public class Teleport : MonoBehaviour
    {
        [SceneName]
        public string sceneToGo;
        public Vector3 positionToGo;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                EventSystem.CallTransitionEvent(sceneToGo, positionToGo);
            }
        }
    }
}

