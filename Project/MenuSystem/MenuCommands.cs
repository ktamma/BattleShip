using System;

namespace MenuSystem
{
    public class MenuCommands
    {
        public string ExitCmd{ get; set; }
        public string ReturnCmd{ get; set; }
        public string MainMenuCmd{ get; set; }
        public MenuCommands(string exitCmd = "X", string returnCmd = "R", string mainMenuCmd = "M")
        {
            ExitCmd = exitCmd.ToLower();
            ReturnCmd = returnCmd.ToLower();
            MainMenuCmd = mainMenuCmd.ToLower();
        }
        private String DefaultMethod()
        {
            return "";
        }

        public String Exit()
        {
            return ExitCmd;
        }

        public String Return()
        {
            return ReturnCmd;
        }

        public String MainMenu()
        {
            return MainMenuCmd;
        }
        
        public String EmptyCmd()
        {
            return "";
        }
        
    }
}