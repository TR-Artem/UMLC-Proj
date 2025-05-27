using System;
using SkinTradingApp.Services;

namespace SkinTradingApp.Services
{
    public class ConsoleService
    {
        private readonly AuthService _authService;
        private readonly TradingService _tradingService;

        public ConsoleService()
        {
            _authService = new AuthService();
            _tradingService = new TradingService();
        }

        public void Run()
        {
            Console.Title = "Торговая площадка скинов";
            
            while (true)
            {
                try
                {
                    Console.Clear();
                    DisplayWelcomeScreen();

                    if (!_authService.IsLoggedIn)
                    {
                        DisplayGuestMenu();
                        ProcessGuestChoice();
                    }
                    else
                    {
                        DisplayUserMenu();
                        ProcessUserChoice();
                    }
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                    Console.ReadKey();
                }
            }
        }

        private void DisplayWelcomeScreen()
        {
            Console.WriteLine("=== ДОБРО ПОЖАЛОВАТЬ НА ТОРГОВУЮ ПЛОЩАДКУ ===");
            Console.WriteLine($"Текущий статус: {(_authService.IsLoggedIn ? $"Авторизован ({_authService.CurrentUser.Email})" : "Гость")}");
            Console.WriteLine(new string('=', 50));
        }

        private void DisplayGuestMenu()
        {
            Console.WriteLine("\nМЕНЮ:");
            Console.WriteLine("1. Войти в систему");
            Console.WriteLine("2. Зарегистрироваться");
            Console.WriteLine("3. Просмотреть доступные скины");
            Console.WriteLine("4. Выйти из программы");
            Console.Write("\nВыберите действие: ");
        }

        private void DisplayUserMenu()
        {
            Console.WriteLine($"\nБаланс: {_authService.CurrentUser.Balance}$");
            Console.WriteLine("\nМЕНЮ:");
            Console.WriteLine("1. Просмотреть мой инвентарь");
            Console.WriteLine("2. Купить скин");
            Console.WriteLine("3. Продать скин");
            Console.WriteLine("4. Выйти из аккаунта");
            Console.WriteLine("5. Выйти из программы");
            Console.Write("\nВыберите действие: ");
        }

        private void ProcessGuestChoice()
        {
            var choice = GetIntInput(1, 4);

            switch (choice)
            {
                case 1:
                    _authService.Login();
                    break;
                case 2:
                    _authService.Register();
                    break;
                case 3:
                    _tradingService.ShowAvailableSkins();
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
            }
        }

        private void ProcessUserChoice()
        {
            var choice = GetIntInput(1, 5);

            switch (choice)
            {
                case 1:
                    _tradingService.ShowUserInventory(_authService.CurrentUser.Id);
                    break;
                case 2:
                    _tradingService.BuySkin(_authService.CurrentUser);
                    break;
                case 3:
                    _tradingService.SellSkin(_authService.CurrentUser);
                    break;
                case 4:
                    _authService.Logout();
                    break;
                case 5:
                    Environment.Exit(0);
                    break;
            }
        }

        private int GetIntInput(int min, int max)
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int result) && result >= min && result <= max)
                {
                    return result;
                }
                Console.Write($"Пожалуйста, введите число от {min} до {max}: ");
            }
        }

        private void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nОшибка: {message}");
            Console.ResetColor();
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
        }
    }
}
