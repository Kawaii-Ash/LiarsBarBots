using HarmonyLib;
using System.Reflection;

namespace LiarsBarBots.Utils
{
    public class DiceGameProxy
    {
        public DiceGamePlay diceGamePlay;

        public static MethodInfo _PlayLiarCMD = AccessTools.Method(typeof(DiceGamePlay), "PlayLiarCMD");
        public static MethodInfo _CallLiar = AccessTools.Method(typeof(DiceGamePlay), "CallLier");

        public static MethodInfo _PlaySpotOnCMD = AccessTools.Method(typeof(DiceGamePlay), "PlaySpotOnCMD");
        public static MethodInfo _CallSpotOn = AccessTools.Method(typeof(DiceGamePlay), "CallSpotOn");

        public static MethodInfo _PlaceBet = AccessTools.Method(typeof(DiceGamePlay), "PlaceBet");
        public static MethodInfo _ResetTurn = AccessTools.Method(typeof(DiceGamePlay), "ResetTurn");
        public static MethodInfo _GiveTurn = AccessTools.Method(typeof(DiceGamePlay), "GiveTurn");

        public DiceGameProxy(DiceGamePlay _diceGamePlay)
        {
            diceGamePlay = _diceGamePlay;
        }

        public void CallLiar()
        {
            _PlayLiarCMD.Invoke(diceGamePlay, []);
            _CallLiar.Invoke(diceGamePlay, []);
        }

        public void CallSpotOn()
        {
            _PlaySpotOnCMD.Invoke(diceGamePlay, []);
            _CallSpotOn.Invoke(diceGamePlay, []);
        }

        public void PlaceBet(int Amount, int Die)
        {
            diceGamePlay.NetworkBetCount = Amount; // Max 20
            diceGamePlay.NetworkBetDice = Die; // 1-6
            _PlaceBet.Invoke(diceGamePlay, []);
            _ResetTurn.Invoke(diceGamePlay, []);
            _GiveTurn.Invoke(diceGamePlay, []);
        }
    }
}
