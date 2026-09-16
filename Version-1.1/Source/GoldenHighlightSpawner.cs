using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Characters;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace Calloatti.HighlightTweaks
{
    internal class GoldenHighlightSpawner : ILoadableSingleton, IPostLoadableSingleton, IUnloadableSingleton
    {
        private readonly EventBus _eventBus;
        private Material _glowMaterial;
        private Texture2D _glowTexture;
        private GameObject _currentEffect;

        private static readonly Color GoldenColor = new Color(1f, 0.85f, 0.2f, 1f);

        public GoldenHighlightSpawner(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Load()
        {
            HighlightableObject_HighlightPrimary_Patch.IsEntitySelected = IsCharacter;

            _glowTexture = new Texture2D(1, 64);
            _glowTexture.wrapMode = TextureWrapMode.Clamp;
            for (int i = 0; i < 64; i++)
            {
                float distFromCenter = Mathf.Abs((i / 63f) - 0.5f) * 2f;
                float alpha = Mathf.Clamp01(1f - (distFromCenter * distFromCenter));
                _glowTexture.SetPixel(0, i, new Color(1, 1, 1, alpha));
            }
            _glowTexture.Apply();

            _glowMaterial = new Material(Shader.Find("Sprites/Default"));
            _glowMaterial.mainTexture = _glowTexture;
            _glowMaterial.SetInt("_ZTest", (int)CompareFunction.Always);
            _glowMaterial.renderQueue = 4000;
        }

        public void PostLoad()
        {
            _eventBus.Register(this);
        }

        public void Unload()
        {
            DestroyEffect();
            if (_glowMaterial != null)
            {
                Object.Destroy(_glowMaterial);
                _glowMaterial = null;
            }
            if (_glowTexture != null)
            {
                Object.Destroy(_glowTexture);
                _glowTexture = null;
            }
        }

        private bool IsCharacter(BaseComponent component)
        {
            return component.GetComponent<Character>() != null;
        }

        [OnEvent]
        public void OnSelectableObjectSelected(SelectableObjectSelectedEvent e)
        {
            if (e.SelectableObject.GetComponent<Character>() == null)
                return;
            SpawnEffect(e.SelectableObject.Transform, e.SelectableObject.GetComponent<CharacterModel>());
        }

        [OnEvent]
        public void OnSelectableObjectUnselected(SelectableObjectUnselectedEvent e)
        {
            DestroyEffect();
        }

        private void SpawnEffect(Transform target, CharacterModel characterModel)
        {
            DestroyEffect();

            _currentEffect = new GameObject("GoldenHighlightEffect");
            _currentEffect.AddComponent<GoldenHighlightEffect>().SetTarget(target, characterModel);

            SpawnGroundRing(_currentEffect.transform);
            SpawnVerticalBeam(_currentEffect.transform);
        }

        private void SpawnGroundRing(Transform parent)
        {
            GameObject ring = new GameObject("GoldenRing");
            ring.transform.SetParent(parent, false);

            LineRenderer lr = ring.AddComponent<LineRenderer>();
            lr.material = _glowMaterial;
            lr.useWorldSpace = false;
            lr.sortingOrder = 32767;
            lr.startWidth = 0.06f;
            lr.endWidth = 0.06f;
            lr.loop = true;
            lr.positionCount = 49;

            float radius = 0.35f;
            for (int i = 0; i < 49; i++)
            {
                float angle = (float)i / 48 * Mathf.PI * 2f;
                lr.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, 0.01f, Mathf.Sin(angle) * radius));
            }

            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(GoldenColor, 0f), new GradientColorKey(GoldenColor, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0f), new GradientAlphaKey(0.8f, 1f) }
            );
            lr.colorGradient = grad;
        }

        private void SpawnVerticalBeam(Transform parent)
        {
            GameObject beam = new GameObject("GoldenBeam");
            beam.transform.SetParent(parent, false);

            LineRenderer lr = beam.AddComponent<LineRenderer>();
            lr.material = _glowMaterial;
            lr.useWorldSpace = false;
            lr.sortingOrder = 32767;
            lr.startWidth = 0.06f;
            lr.endWidth = 0.06f;
            lr.positionCount = 3;
            lr.SetPosition(0, new Vector3(0f, 0.8f, 0f));
            lr.SetPosition(1, new Vector3(0f, 1.8f, 0f));
            lr.SetPosition(2, new Vector3(0f, 32.8f, 0f));

            float fadeT = 1f / 32f;
            var colors = new GradientColorKey[3];
            colors[0] = new GradientColorKey(GoldenColor, 0.0f);
            colors[1] = new GradientColorKey(GoldenColor, fadeT);
            colors[2] = new GradientColorKey(GoldenColor, 1.0f);

            var alphas = new GradientAlphaKey[3];
            alphas[0] = new GradientAlphaKey(0.0f, 0.0f);
            alphas[1] = new GradientAlphaKey(0.8f, fadeT);
            alphas[2] = new GradientAlphaKey(0.8f, 1.0f);

            var gradient = new Gradient();
            gradient.SetKeys(colors, alphas);
            lr.colorGradient = gradient;
        }

        private void DestroyEffect()
        {
            if (_currentEffect != null)
            {
                Object.Destroy(_currentEffect);
                _currentEffect = null;
            }
        }

    }
}
