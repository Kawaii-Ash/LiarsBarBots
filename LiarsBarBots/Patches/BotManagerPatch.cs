using HarmonyLib;
using LiarsBarBots.Components;
using LiarsBarBots.Utils;
using Mirror;
using System.Reflection;
using UnityEngine;

namespace LiarsBarBots.Patches
{
    [HarmonyPatch]
    public class BotManagerPatch
    {
        static FieldInfo ManagerrField = AccessTools.Field(typeof(Manager), "managerr");

        [HarmonyPatch(typeof(Manager), "StartGame")]
        [HarmonyPostfix]
        public static void StartGamePostfix(Manager __instance)
        {
            if (__instance.isServer)
            {
                var managerr = (CustomNetworkManager)ManagerrField.GetValue(__instance);
                var botLobbyManager = managerr.GetComponent<BotLobbyManager>();
                if (botLobbyManager.FillWithBots)
                {
                    var botOwner = __instance.GetLocalPlayer().gameObject;
                    for (int i = managerr.GamePlayers.Count; i < 4; i++)
                    {
                        BotSpawner.SpawnBot(__instance, botOwner);
                    }
                }
                Object.Destroy(botLobbyManager);
            }
        }

        [HarmonyPatch(typeof(Manager), "GetLocalPlayer")]
        [HarmonyPrefix]
        public static void GetLocalPlayerPrefix(ref bool __runOriginal, ref PlayerStats __result)
        {
            __runOriginal = false;
            GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject gameObject in array)
            {
                if (gameObject.GetComponent<PlayerStats>().isOwned)
                {
                    if (gameObject.GetComponent<BotController>() == null)
                    {
                        __result = gameObject.GetComponent<PlayerStats>();
                        return;
                    }
                }
            }
            __result = null;
        }
    }
}
