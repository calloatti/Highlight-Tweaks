using Timberborn.CharacterModelSystem;
using UnityEngine;

namespace Calloatti.HighlightTweaks
{
    public class GoldenHighlightEffect : MonoBehaviour
    {
        private Transform _target;
        private CharacterModel _characterModel;

        public void SetTarget(Transform target, CharacterModel characterModel)
        {
            _target = target;
            _characterModel = characterModel;
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 pos = _characterModel != null ? _characterModel.Position : _target.position;
            transform.position = new Vector3(pos.x, pos.y, pos.z);
        }
    }
}
