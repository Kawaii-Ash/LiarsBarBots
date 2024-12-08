using HarmonyLib;
using LiarsBarBots.Components;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace LiarsBarBots.Patches.BlorfGamePatches
{
    [HarmonyPatch]
    public class GamePlayPatch
    {
        static FieldInfo ManagerField = AccessTools.Field(typeof(BlorfGamePlay), "manager");
        static FieldInfo RevolverSetField = AccessTools.Field(typeof(BlorfGamePlay), "revolverset");
        static FieldInfo playerStatsField = AccessTools.Field(typeof(BlorfGamePlay), "playerStats");
        static FieldInfo RoundTextField = AccessTools.Field(typeof(BlorfGamePlay), "RoundText");
        static FieldInfo CurrentRevolverField = AccessTools.Field(typeof(BlorfGamePlay), "currentrevoler");

        [HarmonyPatch(typeof(BlorfGamePlay), "UpdateCall")]
        [HarmonyPrefix]
        public static void UpdateCallPrefix(BlorfGamePlay __instance, ref bool __runOriginal)
        {
            var isBot = __instance.GetComponent<BotController>() != null;
            if (isBot)
            {
                __runOriginal = false;
                __instance.CardsParent.SetActive(__instance.HaveCards);
                __instance.animator.SetBool("HaveCard", __instance.HaveCards);
                var manager = (Manager)ManagerField.GetValue(__instance);
                var revolverset = (bool)RevolverSetField.GetValue(__instance);
                if (manager.GameStarted && !revolverset)
                {
                    RevolverSetField.SetValue(__instance, true);
                    __instance.Networkrevolverbulllet = UnityEngine.Random.Range(0, 6);
                }

                var playerStats = (PlayerStats)playerStatsField.GetValue(__instance);
                if (playerStats.HaveTurn)
                {
                    __instance.NetworkLooking = true;
                    __instance.animator.SetBool("Look", true);
                }
                else
                {
                    __instance.NetworkLooking = false;
                    __instance.animator.SetBool("Look", false);
                }

                var RoundText = (TextMeshPro)RoundTextField.GetValue(__instance);
                var currentrevolver = (int)CurrentRevolverField.GetValue(__instance);
                RoundText.text = "(" + currentrevolver + "|6)";
            }
        }
    }
}
