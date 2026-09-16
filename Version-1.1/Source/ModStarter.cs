using HarmonyLib;
using Timberborn.ModManagerScene;
using UnityEngine;

namespace Calloatti.HighlightTweaks
{
    public class ModStarter : IModStarter
    {
        private const string HarmonyId = "Calloatti.HighlightTweaks";

        public void StartMod(IModEnvironment modEnvironment)
        {
            new Harmony(HarmonyId).PatchAll();
            Debug.Log("[Highlight Tweaks] Harmony patches applied.");
        }
    }
}
