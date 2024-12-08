using HarmonyLib;
using LiarsBarBots.Components;
using System.Reflection;
using UnityEngine;

namespace LiarsBarBots.Patches
{
    [HarmonyPatch]
    public class BotDicePatch
    {
        static FieldInfo OwnedField = AccessTools.Field(typeof(Dice), "Owned");
        static FieldInfo RendererField = AccessTools.Field(typeof(Dice), "renderer");

        [HarmonyPatch(typeof(Dice), "Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(Dice __instance)
        {
            var isOwned = (bool)OwnedField.GetValue(__instance);

            if (isOwned && Manager.Instance != null)
            {
                if (__instance.transform.root.GetComponent<BotController>() != null)
                {
                    var meshRenderer = (MeshRenderer)RendererField.GetValue(__instance);
                    meshRenderer.material = Manager.Instance.zar2;
                }
            }
        }
    }
}
