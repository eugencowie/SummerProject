namespace SummerProject
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu screen, and gives
    /// the user a chance to configure the game.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        MenuEntry ungulateMenuEntry;
        MenuEntry languageMenuEntry;
        MenuEntry frobnicateMenuEntry;
        MenuEntry elfMenuEntry;

        enum Ungulate {
            BactrianCamel,
            Dromedary,
            Llama
        }

        static Ungulate currentUngulate = Ungulate.Dromedary;

        static string[] languages = { "C#", "French", "Deoxyribonucleic acid" };
        static int currentLanguage;

        static bool frobnicate = true;

        static int elf = 23;

        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            ungulateMenuEntry = new MenuEntry(string.Empty);
            languageMenuEntry = new MenuEntry(string.Empty);
            frobnicateMenuEntry = new MenuEntry(string.Empty);
            elfMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            var back = new MenuEntry("Back");

            // Hook up menu event handlers.
            ungulateMenuEntry.Selected += UngulateMenuEntrySelected;
            languageMenuEntry.Selected += LanguageMenuEntrySelected;
            frobnicateMenuEntry.Selected += FrobnicateMenuEntrySelected;
            elfMenuEntry.Selected += ElfMenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(ungulateMenuEntry);
            MenuEntries.Add(languageMenuEntry);
            MenuEntries.Add(frobnicateMenuEntry);
            MenuEntries.Add(elfMenuEntry);
            MenuEntries.Add(back);
        }

        private void SetMenuEntryText()
        {
            ungulateMenuEntry.Text = "Preferred ungulate: " + currentUngulate;
            languageMenuEntry.Text = "Language: " + languages[currentLanguage];
            frobnicateMenuEntry.Text = "Frobnicate: " + (frobnicate ? "on" : "off");
            elfMenuEntry.Text = "elf: " + elf;
        }

        private void UngulateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentUngulate++;

            if (currentUngulate > Ungulate.Llama)
                currentUngulate = 0;

            SetMenuEntryText();
        }

        private void LanguageMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentLanguage = (currentLanguage + 1) % languages.Length;

            SetMenuEntryText();
        }

        private void FrobnicateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            frobnicate = !frobnicate;

            SetMenuEntryText();
        }

        private void ElfMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            elf++;

            SetMenuEntryText();
        }
    }
}
