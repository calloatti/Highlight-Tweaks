using HarmonyLib;
using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.SelectionSystem;

namespace Calloatti.HighlightTweaks
{
    [HarmonyPatch(typeof(HighlightableObject), nameof(HighlightableObject.HighlightPrimary))]
    internal static class HighlightableObject_HighlightPrimary_Patch
    {
        internal static System.Func<BaseComponent, bool> IsEntitySelected { get; set; }

        static bool Prefix(HighlightableObject __instance)
        {
            if (IsEntitySelected != null && IsEntitySelected(__instance))
                return false;

            return true;
        }
    }
}
