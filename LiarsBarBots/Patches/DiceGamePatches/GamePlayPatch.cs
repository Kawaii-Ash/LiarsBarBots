using HarmonyLib;
using LiarsBarBots.Components;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LiarsBarBots.Patches.DiceGamePatches
{
    

    [HarmonyPatch]
    public class GamePlayPatch
    {
        static FieldInfo ManagerField = AccessTools.Field(typeof(DiceGamePlay), "manager");
        static FieldInfo PlayerStatsField = AccessTools.Field(typeof(DiceGamePlay), "playerStats");
        static FieldInfo ZarKafaSpriteField = AccessTools.Field(typeof(DiceGamePlay), "ZarKafaSprite");
        static FieldInfo ZarTextField = AccessTools.Field(typeof(DiceGamePlay), "ZarText");
        static FieldInfo Zehir1Field = AccessTools.Field(typeof(DiceGamePlay), "Zehir1");
        static FieldInfo Zehir2Field = AccessTools.Field(typeof(DiceGamePlay), "Zehir2");
        static FieldInfo dicerendersField = AccessTools.Field(typeof(DiceGamePlay), "dicerenders");

        [HarmonyPatch(typeof(DiceGamePlay), "WaitforPosion")]
        [HarmonyPostfix]
        static IEnumerator WaitForPosionWrapper(IEnumerator result, DiceGamePlay __instance)
        {
            yield return new WaitForSeconds(2.5f);
            var isBot = __instance.GetComponent<BotController>() != null;
            if (__instance.isOwned && !isBot)
            {
                var manager = (Manager)ManagerField.GetValue(__instance);
                manager.DiceGame.VignetteEf.SetActive(value: true);
            }
            __instance.GetPosinoed();
        }

        [HarmonyPatch(typeof(DiceGamePlay), "UpdateCall")]
        [HarmonyPrefix]
        public static void UpdateCallPrefix(DiceGamePlay __instance, ref bool __runOriginal)
        {
            var isBot = __instance.GetComponent<BotController>() != null;
            if (isBot)
            {
                __runOriginal = false;
                var playerStats = (PlayerStats)PlayerStatsField.GetValue(__instance);

                __instance.animator.SetBool("Dead", playerStats.Dead);
                __instance.animator.SetBool("Show", playerStats.Show);

                var ZarKafaSprite = (GameObject)ZarKafaSpriteField.GetValue(__instance);
                ZarKafaSprite.SetActive(playerStats.HaveTurn);

                var ZarText = (GameObject)ZarTextField.GetValue(__instance);
                ZarText.SetActive(!playerStats.Dead && playerStats.Show);

                if (playerStats.Health == 1)
                {
                    var Zehir1 = (GameObject)Zehir1Field.GetValue(__instance);
                    Zehir1.SetActive(false);
                }
                else if (playerStats.Dead)
                {
                    var Zehir2 = (GameObject)Zehir2Field.GetValue(__instance);
                    Zehir2.gameObject.SetActive(false);
                }

                if (__instance.DiceValues.Count > 0)
                {
                    var dicerenders = (List<Dice>)dicerendersField.GetValue(__instance);
                    for (int i = 0; i < dicerenders.Count; i++)
                    {
                        dicerenders[i].Face = __instance.DiceValues[i];
                    }
                }
            }
        }
    }
}
