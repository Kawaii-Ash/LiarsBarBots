using HarmonyLib;
using LiarsBarBots.Components;
using Mirror;
using System.Reflection;

namespace LiarsBarBots.Patches
{
    [HarmonyPatch]
    public class BotStartPatch
    {
        static FieldInfo ManagerrField = AccessTools.Field(typeof(LobbyController), "managerr");

        [HarmonyPatch(typeof(LobbyController), "Start")]
        [HarmonyPostfix]
        public static void StartPostfix(LobbyController __instance)
        {
            if (__instance.isServer)
            {
                var managerr = NetworkManager.singleton as CustomNetworkManager;
                managerr.gameObject.AddComponent<BotLobbyManager>();
            }
        }

        [HarmonyPatch(typeof(LobbyController), "Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(LobbyController __instance)
        {
            if (__instance.isServer)
            {
                var managerr = (CustomNetworkManager)ManagerrField.GetValue(__instance);
                var botLobby = managerr.GetComponent<BotLobbyManager>();
                if (botLobby != null) {
                    if (botLobby.FillWithBots && managerr.GamePlayers.Count == 1)
                    {
                        __instance.StartButtonActive.SetActive(value: true);
                        __instance.StartButtonPassive.SetActive(value: false);
                    }
                }
            }
        }
    }
}
