using HarmonyLib;
using LiarsBarBots.Components;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LiarsBarBots.Utils
{
    public static class BotSpawner
    {
        static FieldInfo RoulettePlayerPrefabsField = AccessTools.Field(typeof(Manager), "RoulettePlayerPrefabs");
        static FieldInfo DicePlayerPrefabsField = AccessTools.Field(typeof(Manager), "DicePlayerPrefabs");
        static FieldInfo ChaosPlayerPrefabsField = AccessTools.Field(typeof(Manager), "ChaosPlayerPrefabs");

        public static void SpawnBot(Manager manager, GameObject owner)
        {
            if (GetFreeSlot(manager, out var spawnSlot))
            {
                List<GameObject> skinList = new List<GameObject>();
                if (manager.mode == CustomNetworkManager.GameMode.LiarsDeck)
                {
                    skinList = (List<GameObject>)RoulettePlayerPrefabsField.GetValue(manager);
                }
                else if (manager.mode == CustomNetworkManager.GameMode.LiarsDice)
                {
                    skinList = (List<GameObject>)DicePlayerPrefabsField.GetValue(manager);
                }
                else if (manager.mode == CustomNetworkManager.GameMode.LiarsChaos)
                {
                    skinList = (List<GameObject>)ChaosPlayerPrefabsField.GetValue(manager);
                }
                var skin = skinList[UnityEngine.Random.Range(0, skinList.Count)];
                var Slots = manager.Slots;
                Transform botTransform = Slots.Where((Transform x) => x.GetComponent<Slot>().SlotID == spawnSlot).First();
                var player = UnityEngine.Object.Instantiate(skin, botTransform.position, botTransform.rotation);
                var botController = player.AddComponent<BotController>();
                botController.manager = manager;
                NetworkServer.Spawn(player, owner);
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                playerStats.NetworkPlayer_Id = (ulong)botTransform.GetHashCode();
                playerStats.NetworkPlayerName = "Bot";
                playerStats.NetworkSlot = spawnSlot;

                if (manager.DiceGame)
                {
                    manager.DiceGame.NetworkMaxCount = manager.DiceGame.MaxCount + 5;
                }
                manager.RefreshPlayerList();
            }
        }

        static bool GetFreeSlot(Manager manager, out int freeSlot)
        {
            freeSlot = 0;
            if (manager.Players.Count == 4) return false;

            var availableSlots = new List<int>() { 0, 1, 2, 3 };
            foreach (var player in manager.Players)
            {
                availableSlots.Remove(player.Slot);
            }

            if (availableSlots.Count > 0)
            {
                freeSlot = availableSlots[0];
                return true;
            }

            return false;
        }
    }
}
