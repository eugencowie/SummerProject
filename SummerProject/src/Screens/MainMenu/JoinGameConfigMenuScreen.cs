using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SummerProject
{
    class JoinGameConfigMenuScreen : MenuScreen
    {
        MenuEntry typeMenuEntry;

        string[] types = {
            "Warrior",
            "Tank",
            "Mage",
            "Support"
        };
        int currentType = 0;


        /// <summary>
        /// Constructor.
        /// </summary>
        public JoinGameConfigMenuScreen(string address, int port)
            : base("Character Customisation")
        {
            // Create menu entries.
            typeMenuEntry = new MenuEntry(string.Empty);
            var join = new MenuEntry("Join");
            var cancel = new MenuEntry("Cancel");
            SetMenuEntryText();

            // Hook up menu event handlers.
            typeMenuEntry.Selected += (sender, e) => {
                currentType = (currentType + 1) % types.Length;
                SetMenuEntryText();
            };

            typeMenuEntry.Increased += (sender, e) => {
                currentType = (currentType + 1) % types.Length;
                SetMenuEntryText();
            };

            typeMenuEntry.Decreased += (sender, e) => {
                currentType = currentType - 1;
                if (currentType == -1)
                    currentType = types.Length - 1;
                SetMenuEntryText();
            };

            join.Selected += (sender, e) => {
                NetworkingSystem.Client.Connect(address, port, () => {
                    LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new GameplayScreen((PlayerType)currentType));
                });
            };

            cancel.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(typeMenuEntry);
            MenuEntries.Add(join);
            MenuEntries.Add(cancel);
        }


        /// <summary>
        /// Fills in the latest values for the menu entries.
        /// </summary>
        private void SetMenuEntryText()
        {
            typeMenuEntry.Text = string.Format("Type: {0}", types[currentType]);
        }
    }
}
