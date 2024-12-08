using HarmonyLib;
using LiarsBarBots.Components;
using System.Reflection;
using TMPro;

namespace LiarsBarBots.Patches
{
    [HarmonyPatch]
    public class BotPlayerStatsPatch
    {
        static FieldInfo NameTextField = AccessTools.Field(typeof(PlayerStats), "NameText");

        [HarmonyPatch(typeof(PlayerStats), "Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(PlayerStats __instance)
        {
            var isBot = __instance.GetComponent<BotController>() != null;
            if (isBot)
            {
                var nameText = (TextMeshPro)NameTextField.GetValue(__instance);
                nameText.transform.parent.gameObject.SetActive(true);
            }
        }
    }
}
