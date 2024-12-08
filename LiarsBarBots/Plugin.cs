using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LiarsBarBots.Patches;
using BlorfGamePatches = LiarsBarBots.Patches.BlorfGamePatches;
using ChaosGamePatches = LiarsBarBots.Patches.ChaosGamePatches;
using DiceGamePatches = LiarsBarBots.Patches.DiceGamePatches;

namespace LiarsBarBots
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            Harmony.CreateAndPatchAll(typeof(BotStartPatch), nameof(BotStartPatch));
            Harmony.CreateAndPatchAll(typeof(BotManagerPatch), nameof(BotManagerPatch));
            Harmony.CreateAndPatchAll(typeof(BotCharControllerPatch), nameof(BotCharControllerPatch));
            Harmony.CreateAndPatchAll(typeof(BotPlayerStatsPatch), nameof(BotPlayerStatsPatch));
            Harmony.CreateAndPatchAll(typeof(BotCardPatch), nameof(BotCardPatch));
            Harmony.CreateAndPatchAll(typeof(BotDicePatch), nameof(BotDicePatch));

            Harmony.CreateAndPatchAll(typeof(BlorfGamePatches.GamePlayPatch), nameof(BlorfGamePatches.GamePlayPatch));
            Harmony.CreateAndPatchAll(typeof(BlorfGamePatches.GamePlayManagerPatch), nameof(BlorfGamePatches.GamePlayManagerPatch));

            Harmony.CreateAndPatchAll(typeof(ChaosGamePatches.GamePlayPatch), nameof(ChaosGamePatches.GamePlayPatch));
            Harmony.CreateAndPatchAll(typeof(ChaosGamePatches.GamePlayManagerPatch), nameof(ChaosGamePatches.GamePlayManagerPatch));

            Harmony.CreateAndPatchAll(typeof(DiceGamePatches.GamePlayPatch), nameof(DiceGamePatches.GamePlayPatch));
            Harmony.CreateAndPatchAll(typeof(DiceGamePatches.GamePlayManagerPatch), nameof(DiceGamePatches.GamePlayManagerPatch));

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
