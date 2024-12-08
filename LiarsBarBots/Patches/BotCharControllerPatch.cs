using HarmonyLib;
using LiarsBarBots.Components;
using System.Reflection;
using UnityEngine;
using System.Linq;
using LiarsBarBots.Utils;

namespace LiarsBarBots.Patches
{
    [HarmonyPatch]
    public class BotCharControllerPatch
    {
        static FieldInfo PlayerStatsField = AccessTools.Field(typeof(CharController), "playerStats");
        static FieldInfo ManagerField = AccessTools.Field(typeof(CharController), "manager");

        [HarmonyPatch(typeof(CharController), "Start")]
        [HarmonyPostfix]
        public static void StartPostfix(CharController __instance)
        {
            var isBot = __instance.GetComponent<BotController>() != null;
            if (__instance.isOwned && isBot)
            {
                __instance.VRCamera.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(CharController), "Update")]
        [HarmonyPrefix]
        public static void UpdatePrefix(CharController __instance, ref bool __runOriginal)
        {
            var isBot = __instance.GetComponent<BotController>() != null;
            if (__instance.isOwned && isBot)
            {
                __runOriginal = false;
                var UpdateCallMethod = AccessTools.Method(typeof(CharController), "UpdateCall");
                UpdateCallMethod.Invoke(__instance, []);

                var playerStats = (PlayerStats)PlayerStatsField.GetValue(__instance);
                if (playerStats.Winner)
                {
                    var manager = (Manager)ManagerField.GetValue(__instance);
                    manager.SpectatorCameraParrent.SetActive(value: false);
                    manager.Slots.Where((Transform x) => x.GetComponent<Slot>().SlotID == playerStats.Slot).First().GetComponent<Slot>()
                        .Cameraa.SetActive(true);
                }
            }
        }

        [HarmonyPatch(typeof(CharController), "Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(CharController __instance)
        {
            if (__instance.isOwned && __instance.isServer)
            {
                var isBot = __instance.GetComponent<BotController>() != null;
                if (!isBot)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0))
                    {
                        var manager = (Manager)ManagerField.GetValue(__instance);
                        BotSpawner.SpawnBot(manager, __instance.gameObject);
                    }
                }
            }
        }
    }
}
