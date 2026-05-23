using System.Reflection;

namespace PluginApp
{
    // Интерфейс плагина
    public interface IPlugin
    {
        string Name { get; }
        void Execute();
    }

    // Плагин 1: Приветствие
    public class HelloPlugin : IPlugin
    {
        public string Name => "Приветствие";
        public void Execute() => Console.WriteLine("👋 Привет из плагина!");
    }

    // Плагин 2: Калькулятор
    public class MathPlugin : IPlugin
    {
        public string Name => "Калькулятор";
        public void Execute()
        {
            Console.Write("Введите число A: ");
            int a = int.Parse(Console.ReadLine());
            Console.Write("Введите число B: ");
            int b = int.Parse(Console.ReadLine());
            Console.WriteLine($"Результат: {a + b}");
        }
    }

    // Плагин 3: Время
    public class TimePlugin : IPlugin
    {
        public string Name => "Текущее время";
        public void Execute() => Console.WriteLine($"🕐 {DateTime.Now:HH:mm:ss}");
    }

    public class JokePlugin : IPlugin
    {
        public string Name => "Шутка дня";
        public void Execute() => Console.WriteLine("Почему программисты любят dark theme? Потому что свет их бесит!");
    }
    class Program
    {
        static List<IPlugin> plugins = new List<IPlugin>();

        static void Main()
        {
            // Регистрируем плагины вручную (в одном проекте не нужна DLL загрузка)
            plugins.Add(new HelloPlugin());
            plugins.Add(new MathPlugin());
            plugins.Add(new TimePlugin());
            plugins.Add(new JokePlugin());


            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Модульное приложение ===\n");

                // Показываем все плагины
                for (int i = 0; i < plugins.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {plugins[i].Name}");
                }
                Console.WriteLine($"\n0. Выход");

                Console.Write("\nВыберите функцию: ");
                string input = Console.ReadLine();

                if (input == "0") break;

                if (int.TryParse(input, out int choice) && choice > 0 && choice <= plugins.Count)
                {
                    Console.WriteLine("\n--- Выполнение ---");
                    plugins[choice - 1].Execute();
                    Console.WriteLine("\nНажмите любую клавишу...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Неверный выбор!");
                    Console.ReadKey();
                }
            }
        }
    }
}