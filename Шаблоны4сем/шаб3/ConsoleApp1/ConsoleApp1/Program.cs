using System;
using System.Collections.Generic;
using System.Threading;

namespace SimpleCacheSystem
{
    // 1. БАЗОВАЯ НАСТРОЙКА: основной источник данных (имитация БД)
    public class DataSource
    {
        private int _dbCallCount = 0;

        public string GetData(int id)
        {
            _dbCallCount++;
            Console.WriteLine($"[БАЗА ДАННЫХ] Выполнен трудоемкий запрос для ID={id} (всего вызовов: {_dbCallCount})");
            Thread.Sleep(500); // Имитация задержки БД
            return $"Данные для ID={id} (версия от {DateTime.Now:ss})";
        }

        public int GetCallCount() => _dbCallCount;
    }

    // 2. СТРУКТУРА КЭШИРОВАНИЯ: простой кэш с истечением срока
    public class SimpleCache
    {
        private class CacheItem
        {
            public string Data { get; set; }
            public DateTime ExpiryTime { get; set; }
        }

        private Dictionary<int, CacheItem> _cache = new Dictionary<int, CacheItem>();
        private TimeSpan _ttl; // Time To Live (время жизни кэша)

        public SimpleCache(int ttlSeconds = 10)
        {
            _ttl = TimeSpan.FromSeconds(ttlSeconds);
        }

        public void Add(int key, string data)
        {
            _cache[key] = new CacheItem
            {
                Data = data,
                ExpiryTime = DateTime.Now.Add(_ttl)
            };
            Console.WriteLine($"[КЭШ] Данные для ID={key} добавлены в кэш (истекает через {_ttl.TotalSeconds} сек)");
        }

        public bool TryGet(int key, out string data)
        {
            if (_cache.TryGetValue(key, out CacheItem item))
            {
                if (DateTime.Now < item.ExpiryTime)
                {
                    data = item.Data;
                    Console.WriteLine($"[КЭШ] HIT: Данные для ID={key} найдены в кэше");
                    return true;
                }
                else
                {
                    Console.WriteLine($"[КЭШ] MISS (истек срок): Данные для ID={key} удалены из кэша");
                    _cache.Remove(key);
                }
            }

            data = null;
            Console.WriteLine($"[КЭШ] MISS: Данные для ID={key} отсутствуют в кэше");
            return false;
        }

        public void Remove(int key)
        {
            if (_cache.Remove(key))
                Console.WriteLine($"[КЭШ] Данные для ID={key} удалены из кэша");
        }

        public void Clear()
        {
            _cache.Clear();
            Console.WriteLine("[КЭШ] Кэш полностью очищен");
        }
    }

    // 3. РЕАЛИЗАЦИЯ ПРОКСИ: перехватывает запросы и проверяет кэш
    public class DataProxy
    {
        private DataSource _dataSource;
        private SimpleCache _cache;

        public DataProxy(DataSource dataSource, SimpleCache cache)
        {
            _dataSource = dataSource;
            _cache = cache;
        }

        public string GetData(int id)
        {
            Console.WriteLine($"\n--- ЗАПРОС: получить данные для ID={id} ---");

            // Сначала проверяем кэш
            if (_cache.TryGet(id, out string cachedData))
            {
                return cachedData;
            }

            // Если нет в кэше - идем в источник данных
            string freshData = _dataSource.GetData(id);
            _cache.Add(id, freshData);
            return freshData;
        }

        public void InvalidateCache(int id)
        {
            _cache.Remove(id);
        }

        public int GetDatabaseCallCount() => _dataSource.GetCallCount();
    }

    // 4. ТЕСТИРОВАНИЕ
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== СИСТЕМА КЭШИРОВАНИЯ ДАННЫХ ===\n");

            // Инициализация компонентов
            var dataSource = new DataSource();
            var cache = new SimpleCache(ttlSeconds: 5); // Кэш живет 5 секунд
            var proxy = new DataProxy(dataSource, cache);

            // ТЕСТ 1: Первое получение данных (должно идти в БД)
            Console.WriteLine("ТЕСТ 1: Первый запрос данных");
            Console.WriteLine($"Результат: {proxy.GetData(1)}");

            // ТЕСТ 2: Повторный запрос тех же данных (должен быть из кэша)
            Console.WriteLine("\nТЕСТ 2: Повторный запрос тех же данных");
            Console.WriteLine($"Результат: {proxy.GetData(1)}");

            // ТЕСТ 3: Запрос других данных (должен идти в БД)
            Console.WriteLine("\nТЕСТ 3: Запрос других данных");
            Console.WriteLine($"Результат: {proxy.GetData(2)}");

            // ТЕСТ 4: Повторный запрос других данных (из кэша)
            Console.WriteLine("\nТЕСТ 4: Повторный запрос данных ID=2");
            Console.WriteLine($"Результат: {proxy.GetData(2)}");

            // ТЕСТ 5: Проверка истечения срока кэша
            Console.WriteLine("\nТЕСТ 5: Ожидание истечения срока кэша (6 секунд)...");
            Thread.Sleep(6000);
            Console.WriteLine($"Результат: {proxy.GetData(1)}");

            // ИТОГИ
            Console.WriteLine("\n=== СТАТИСТИКА ===");
            Console.WriteLine($"Всего обращений к базе данных: {proxy.GetDatabaseCallCount()}");
            Console.WriteLine($"Запросов выполнено: 5");
            Console.WriteLine($"Кэш сэкономил: {5 - proxy.GetDatabaseCallCount()} обращений к БД");

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}