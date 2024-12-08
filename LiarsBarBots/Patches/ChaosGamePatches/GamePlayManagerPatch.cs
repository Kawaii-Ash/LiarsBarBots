﻿using HarmonyLib;
using LiarsBarBots.Components;
using System.Reflection;
using System.Collections;
using UnityEngine;

namespace LiarsBarBots.Patches.ChaosGamePatches
{
    [HarmonyPatch]
    public class GamePlayManagerPatch
    {
        static FieldInfo ManagerField = AccessTools.Field(typeof(ChaosGamePlayManager), "manager");

        [HarmonyPatch(typeof(ChaosGamePlayManager), "Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(ChaosGamePlayManager __instance)
        {
            var manager = (Manager)ManagerField.GetValue(__instance);
            var GetTargetPlayerMethod = AccessTools.Method(typeof(Manager), "GetTargetPlayer", [typeof(int), typeof(bool)]);
            PlayerStats targetPlayer = (PlayerStats)GetTargetPlayerMethod.Invoke(manager, [manager.ActivePlayerSlot, false]);
            var isBot = targetPlayer.GetComponent<BotController>() != null;
            if (isBot)
            {
                __instance.SlotOkAnimator.GetComponent<SpriteRenderer>().color = manager.yellow;
            }
        }

        [HarmonyPatch(typeof(ChaosGamePlayManager), "WaitforNextRound")]
        [HarmonyPostfix]
        static IEnumerator WaitForNextRoundWrapper(IEnumerator result, DiceGamePlayManager __instance)
        {
            var manager = (Manager)ManagerField.GetValue(__instance);
            for (int i = 0; i < manager.Players.Count; i++)
            {
                var botController = manager.Players[i].GetComponent<BotController>();
                if (botController != null)
                {
                    botController.canPlay = false;
                }
            }

            while (result.MoveNext())
                yield return result.Current;

            for (int i = 0; i < manager.Players.Count; i++)
            {
                var botController = manager.Players[i].GetComponent<BotController>();
                if (botController != null)
                {
                    botController.canPlay = true;
                }
            }
        }
    }
}
