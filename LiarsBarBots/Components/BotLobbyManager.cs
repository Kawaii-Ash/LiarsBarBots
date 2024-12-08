using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LiarsBarBots.Components
{
    public class BotLobbyManager : MonoBehaviour
    {
        public bool FillWithBots = false;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert)) FillWithBots = !FillWithBots;
        }

        private void OnGUI()
        {
            var scaleMultiplier = Screen.height / 1080f;
            var fontSize = (int)(17 * scaleMultiplier);
            GUIStyle hintStyle = new GUIStyle
            {
                fontSize = fontSize,
                normal = { textColor = Color.white },
            };

            GUI.Label(new Rect(Screen.width - (Screen.width / 3), Screen.height - fontSize - 10, 100, 20),
                          $"[Toggle: Ins] Fill with Bots: {FillWithBots}",
                          hintStyle);
        }
    }
}