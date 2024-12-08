using HarmonyLib;
using System.Reflection;

namespace LiarsBarBots.Utils
{
    public class ChaosGameProxy
    {
        public ChaosGamePlay chaosGamePlay;

        public static MethodInfo _ThrowCards = AccessTools.Method(typeof(ChaosGamePlay), "ThrowCards");

        public static MethodInfo _PlayLiarCMD = AccessTools.Method(typeof(ChaosGamePlay), "PlayLiarCMD");
        public static MethodInfo _CallLiar = AccessTools.Method(typeof(ChaosGamePlay), "CallLiar");

        public ChaosGameProxy(ChaosGamePlay _chaosGamePlay)
        {
            chaosGamePlay = _chaosGamePlay;
        }

        public void ThrowCards()
        {
            _ThrowCards.Invoke(chaosGamePlay, []);
        }

        public void CallLiar()
        {
            _PlayLiarCMD.Invoke(chaosGamePlay, []);
            _CallLiar.Invoke(chaosGamePlay, []);
        }
    }
}
