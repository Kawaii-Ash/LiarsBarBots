using HarmonyLib;
using LiarsBarBots.Components;
using System.Reflection;
using TMPro;

namespace LiarsBarBots.Patches.ChaosGamePatches
{
    [HarmonyPatch]
    public class GamePlayPatch
    {
        static FieldInfo ManagerField = AccessTools.Field(typeof(ChaosGamePlay), "manager");
        static FieldInfo RevolverSetField = AccessTools.Field(typeof(ChaosGamePlay), "revolverset");
        static FieldInfo playerStatsField = AccessTools.Field(typeof(ChaosGamePlay), "playerStats");
        static FieldInfo RoundTextField = AccessTools.Field(typeof(ChaosGamePlay), "RoundText");
        static FieldInfo CurrentRevolverField = AccessTools.Field(typeof(ChaosGamePlay), "currentrevoler");

        [HarmonyPatch(typeof(ChaosGamePlay), "UpdateCall")]
        [HarmonyPrefix]
        public static void UpdateCallPrefix(ChaosGamePlay __instance, ref bool __runOriginal)
        {
            var isBot = __instance.GetComponent<BotController>() != null;
            if (isBot)
            {
                __runOriginal = false;
                if (__instance.NetworkTakingAim && !__instance.NetworkAimLocked)
                {
                    __instance.animator.SetBool("TakeAim", true);
                }
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
                __instance.NetworkLooking = playerStats.HaveTurn;
                __instance.animator.SetBool("Look", playerStats.HaveTurn);

                var RoundText = (TextMeshPro)RoundTextField.GetValue(__instance);
                var currentrevolver = (int)CurrentRevolverField.GetValue(__instance);
                RoundText.text = "(" + currentrevolver + "|6)";
            }
        }
    }
}
