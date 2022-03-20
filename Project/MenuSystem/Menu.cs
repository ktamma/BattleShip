using System;
using System.Collections.Generic;
using System.Linq;


namespace MenuSystem
{
    public class Menu
    {
        public enum MenuLevel
        {
            Level0,
            Level1,
            Level1Plus
        }

        public static void Main(string[] args)
        {
        }

        private readonly MenuLevel _menuLevel;
        private readonly string _title;
        private readonly MenuCommands _reservedActions;
        private List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        public Menu(MenuLevel menuLevel, MenuCommands reservedActions, string title = "Menu")
        {
            _menuLevel = menuLevel;
            _reservedActions = reservedActions;
            _title = title;
        }


        public string RunMenu()
        {
            Console.Clear();


            Console.CursorVisible = false;

            var menuState = new State(0, MenuItems.Count);
            var consoleState = new State(1, MenuItems.Count + 1);
            string userChoice;

            do
            {
                Console.Clear();

                Console.WriteLine(_title);

                for (var i = 0; i < MenuItems.Count; i++)
                {
                    if (i == menuState.GetState())
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(MenuItems[i] + "<-");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(MenuItems[i].ToString());
                    }
                }



                var pressedKey = Console.ReadKey().Key;
                
                var firstKeyPress = true;
                do
                {
                    if (!firstKeyPress)
                    {
                        pressedKey = Console.ReadKey().Key;
                    }

                    firstKeyPress = false;
                    Console.ResetColor();
                    ConsoleEx.ClearAt(consoleState.GetState());
                    ConsoleEx.WriteTo(consoleState.GetState(), MenuItems[menuState.GetState()].ToString());
                    switch (pressedKey)
                    {
                        case ConsoleKey.DownArrow:
                            menuState.IncreaseState();
                            consoleState.IncreaseState();
                            break;
                        case ConsoleKey.UpArrow:
                            menuState.DecreaseState();
                            consoleState.DecreaseState();
                            break;
                    }
                    
                    ConsoleEx.ClearAt(consoleState.GetState());
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleEx.WriteTo(consoleState.GetState(), MenuItems[menuState.GetState()] + "<-");
                    Console.ResetColor();
                    
                } while (pressedKey != ConsoleKey.Enter);

                userChoice = MenuItems[menuState.GetState()].MethodToExecute().ToLower();


                if (userChoice == _reservedActions.ExitCmd)
                {
                    if (_menuLevel == MenuLevel.Level0)
                    {
                        Console.Clear();
                    }

                    // Console.Clear();

                    break;
                }

                if (_menuLevel != MenuLevel.Level0 && userChoice == _reservedActions.MainMenuCmd)
                {
                    // Console.Clear();

                    break;
                }

                if (_menuLevel == MenuLevel.Level1Plus && userChoice == _reservedActions.ReturnCmd)
                {
                    userChoice = _reservedActions.EmptyCmd();

                    break;
                }

                
            } while (true);
            Console.Clear();

            
            return userChoice;
        }

        public void AddMenuItem(MenuItem item)
        {
            if (MenuItems.Any(menuItem => menuItem.Equals(item)))
            {
                throw new ArgumentException($"Item already exists: {item}");
            }
            
            MenuItems.Add(item);
        }
    }
}