using HarmonyLib;
using LiarsBarBots.Components;
using Mirror;
using UnityEngine;

namespace LiarsBarBots.Patches
{
    [HarmonyPatch]
    public class BotCardPatch
    {
        [HarmonyPatch(typeof(Card), "Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(Card __instance)
        {
            var netId = __instance.transform.root.GetComponent<NetworkIdentity>();
            if (netId != null && netId.isOwned)
            {
                var isBot = netId.GetComponent<BotController>() != null;
                if (isBot)
                {
                    __instance.GetComponent<MeshRenderer>().material = __instance.blurlu;
                }
            }
        }
    }
}
