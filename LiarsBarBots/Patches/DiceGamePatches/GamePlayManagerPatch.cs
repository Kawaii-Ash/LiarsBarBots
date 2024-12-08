using HarmonyLib;
using LiarsBarBots.Components;
using System.Reflection;
using UnityEngine;

namespace LiarsBarBots.Patches.DiceGamePatches
{
    [HarmonyPatch]
    public class GamePlayManagerPatch
    {
        static FieldInfo ManagerField = AccessTools.Field(typeof(DiceGamePlayManager), "manager");

        [HarmonyPatch(typeof(DiceGamePlayManager), "Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(DiceGamePlayManager __instance)
        {
            var manager = (Manager)ManagerField.GetValue(__instance);
            var GetTargetPlayerMethod = AccessTools.Method(typeof(Manager), "GetTargetPlayer", [typeof(int), typeof(bool)]);
            PlayerStats targetPlayer = (PlayerStats)GetTargetPlayerMethod.Invoke(manager, [manager.ActivePlayerSlot, false]);
            var isBot = targetPlayer.GetComponent<BotController>() != null;
            if (isBot)
            {
                __instance.SlotOkAnimator.GetComponent<SpriteRenderer>().color = manager.yellow;
                __instance.TurnNameText.color = Color.white;
                __instance.TurnNameText.text = targetPlayer.PlayerName.Substring(0, Mathf.Min(targetPlayer.PlayerName.Length, 11)) + "'s";
            }
        }
    }
}
