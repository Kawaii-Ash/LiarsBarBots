using HarmonyLib;
using System.Reflection;

namespace LiarsBarBots.Utils
{
    public class DeckGameProxy
    {
        public BlorfGamePlay blorfGamePlay;

        public static MethodInfo _ThrowCards = AccessTools.Method(typeof(BlorfGamePlay), "ThrowCards");

        public static MethodInfo _PlayLiarCMD = AccessTools.Method(typeof(BlorfGamePlay), "PlayLiarCMD");
        public static MethodInfo _CallLiar = AccessTools.Method(typeof(BlorfGamePlay), "CallLiar");

        public DeckGameProxy(BlorfGamePlay _blorfGamePlay)
        {
            blorfGamePlay = _blorfGamePlay;
        }

        public void ThrowCards()
        {
            _ThrowCards.Invoke(blorfGamePlay, []);
        }

        public void CallLiar()
        {
            _PlayLiarCMD.Invoke(blorfGamePlay, []);
            _CallLiar.Invoke(blorfGamePlay, []);
        }
    }
}
