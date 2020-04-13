using Passer;
using UnityEngine;

namespace VrVolleyball
{
    public class HumanoidExternalSettings : MonoBehaviour
    {
        [SerializeField] private HumanoidControl _humanoid;

        [Space(5f)]
        [SerializeField] private bool _isTranslateByPhysics = true;

        private void Awake()
        {
            _humanoid.IsTranslateByPhysics = _isTranslateByPhysics;
        }
    }

}