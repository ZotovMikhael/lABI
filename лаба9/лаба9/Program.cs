using System;
using System.Collections.Generic;

namespace Лабораторная_работа_9
{
    // Базовый абстрактный класс уведомления
    public abstract class Notification : IComparable<Notification>
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string Sender { get; set; }

        public Notification(string message, string sender)
        {
            Message = message;
            Sender = sender;
            Timestamp = DateTime.Now;
        }

        public abstract void Display();

        public int CompareTo(Notification? other)
        {
            if (other == null) return 1;
            return Timestamp.CompareTo(other.Timestamp);
        }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {Sender}: {Message}";
        }
    }

    // Класс Email-уведомления
    public class EmailNotification : Notification
    {
        public string EmailAddress { get; set; }
        public string Subject { get; set; }

        public EmailNotification(string subject, string message, string sender, string emailAddress)
            : base(message, sender)
        {
            Subject = subject;
            EmailAddress = emailAddress;
        }

        public override void Display()
        {
            Console.WriteLine($"Email Notification:");
            Console.WriteLine($"  To: {EmailAddress}");
            Console.WriteLine($"  Subject: {Subject}");
            Console.WriteLine($"  Content: {Message}");
            Console.WriteLine($"  From: {Sender}");
            Console.WriteLine($"  Time: {Timestamp:HH:mm:ss}");
        }
    }

    // Класс SMS-уведомления
    public class SMSNotification : Notification
    {
        public string PhoneNumber { get; set; }

        public SMSNotification(string message, string sender, string phoneNumber)
            : base(message, sender)
        {
            PhoneNumber = phoneNumber;
        }

        public override void Display()
        {
            Console.WriteLine($"SMS Notification:");
            Console.WriteLine($"  To: {PhoneNumber}");
            Console.WriteLine($"  Content: {Message}");
            Console.WriteLine($"  From: {Sender}");
            Console.WriteLine($"  Time: {Timestamp:HH:mm:ss}");
        }
    }

    // Класс Push-уведомления
    public class PushNotification : Notification
    {
        public string DeviceId { get; set; }
        public string AppName { get; set; }

        public PushNotification(string message, string sender, string deviceId, string appName)
            : base(message, sender)
        {
            DeviceId = deviceId;
            AppName = appName;
        }

        public override void Display()
        {
            Console.WriteLine($"Push Notification:");
            Console.WriteLine($"  App: {AppName}");
            Console.WriteLine($"  Device: {DeviceId}");
            Console.WriteLine($"  Content: {Message}");
            Console.WriteLine($"  From: {Sender}");
            Console.WriteLine($"  Time: {Timestamp:HH:mm:ss}");
        }
    }

    // Обобщенный класс контейнера с ограничениями
    public class NotificationContainer<T> where T : Notification, IComparable<T>
    {
        private List<T> _notifications = new List<T>();

        // Ковариантный интерфейс
        public interface ICovariantContainer<out U> where U : Notification
        {
            U GetNotification(int index);
            int Count { get; }
        }

        // Контравариантный интерфейс
        public interface IContravariantContainer<in U> where U : Notification
        {
            void AddNotification(U notification);
        }

        // Добавление уведомления
        public void Add(T notification)
        {
            _notifications.Add(notification);
        }

        // Получение уведомления по индексу
        public T Get(int index)
        {
            if (index >= 0 && index < _notifications.Count)
                return _notifications[index];
            throw new IndexOutOfRangeException();
        }

        // Проверка наличия элементов
        public bool HasElements()
        {
            return _notifications.Count > 0;
        }

        // Количество элементов
        public int Count => _notifications.Count;

        // Сортировка элементов
        public void Sort()
        {
            _notifications.Sort();
        }

        // Вывод всех элементов
        public void DisplayAll()
        {
            foreach (var notification in _notifications)
            {
                notification.Display();
                Console.WriteLine("---");
            }
        }
    }

    // Класс для демонстрации ковариантности и контравариантности
    public static class VariantDemo
    {
        // Демонстрация ковариантности
        public static void DemonstrateCovariance()
        {
            Console.WriteLine("\n=== Демонстрация ковариантности ===");

            NotificationContainer<SMSNotification> smsContainer = new NotificationContainer<SMSNotification>();
            smsContainer.Add(new SMSNotification("Привет!", "Алиса", "+79991234567"));
            smsContainer.Add(new SMSNotification("Встреча завтра", "Босс", "+79998765432"));

            // Ковариантность: NotificationContainer<SMSNotification> может быть приведен к
            // ICovariantContainer<Notification>
            var covariantContainer = smsContainer as NotificationContainer<SMSNotification>.ICovariantContainer<Notification>;

            if (covariantContainer != null)
            {
                Console.WriteLine($"Всего уведомлений: {covariantContainer.Count}");
                var notification = covariantContainer.GetNotification(0);
                Console.WriteLine($"Первое уведомление: {notification}");
            }
        }

        // Демонстрация контравариантности
        public static void DemonstrateContravariance()
        {
            Console.WriteLine("\n=== Демонстрация контравариантности ===");

            NotificationContainer<Notification> generalContainer = new NotificationContainer<Notification>();

            // Контравариантность: можно добавить SMSNotification в контейнер для Notification
            var contravariantContainer = generalContainer as NotificationContainer<Notification>.IContravariantContainer<SMSNotification>;

            if (contravariantContainer != null)
            {
                contravariantContainer.AddNotification(
                    new SMSNotification("Тестовое SMS", "Система", "+70000000000"));
                Console.WriteLine("SMS успешно добавлено через контравариантный интерфейс");
                Console.WriteLine($"Теперь в общем контейнере: {generalContainer.Count} уведомлений");
            }
        }
    }

    // Основной класс программы
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторная работа №9: Обобщения и ограничения ===");
            Console.WriteLine("Тема: Обобщения, ограничение обобщений, наследование обобщенных типов\n");

            // 1. Создание контейнеров разных типов
            Console.WriteLine("1. Создание контейнеров разных типов:");
            var emailContainer = new NotificationContainer<EmailNotification>();
            var smsContainer = new NotificationContainer<SMSNotification>();
            var pushContainer = new NotificationContainer<PushNotification>();

            // 2. Добавление элементов
            Console.WriteLine("\n2. Добавление элементов в контейнеры:");

            emailContainer.Add(new EmailNotification("Важное сообщение", "Проверьте почту", "Система", "admin@example.com"));
            emailContainer.Add(new EmailNotification("Напоминание", "Не забудьте о встрече", "Календарь", "user@example.com"));

            smsContainer.Add(new SMSNotification("Ваш код: 1234", "Банк", "+79161234567"));
            smsContainer.Add(new SMSNotification("Заказ доставлен", "Магазин", "+79031234567"));

            pushContainer.Add(new PushNotification("Новое сообщение", "Мессенджер", "device123", "Telegram"));
            pushContainer.Add(new PushNotification("Обновление готово", "Система", "device456", "OS Updater"));

            // 3. Проверка наличия элементов
            Console.WriteLine("\n3. Проверка наличия элементов:");
            Console.WriteLine($"Email контейнер содержит элементы: {emailContainer.HasElements()}");
            Console.WriteLine($"SMS контейнер содержит элементы: {smsContainer.HasElements()}");
            Console.WriteLine($"Push контейнер содержит элементы: {pushContainer.HasElements()}");

            // 4. Получение и вывод элементов
            Console.WriteLine("\n4. Вывод элементов из контейнеров:");

            Console.WriteLine("\nEmail уведомления:");
            emailContainer.DisplayAll();

            Console.WriteLine("\nSMS уведомления:");
            smsContainer.DisplayAll();

            Console.WriteLine("\nPush уведомления:");
            pushContainer.DisplayAll();

            // 5. Сортировка элементов
            Console.WriteLine("\n5. Сортировка элементов (по времени):");
            emailContainer.Sort();
            Console.WriteLine("Email уведомления после сортировки:");
            emailContainer.DisplayAll();

            // 6. Демонстрация ковариантности и контравариантности
            VariantDemo.DemonstrateCovariance();
            VariantDemo.DemonstrateContravariance();

            // 7. Работа с элементами через базовый класс
            Console.WriteLine("\n7. Работа через базовый класс Notification:");
            NotificationContainer<Notification> generalContainer = new NotificationContainer<Notification>();
            generalContainer.Add(new EmailNotification("Общее", "Это общее уведомление", "Система", "test@example.com"));
            generalContainer.Add(new SMSNotification("Общее SMS", "Система", "+79000000000"));

            Console.WriteLine("Общий контейнер содержит:");
            for (int i = 0; i < generalContainer.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {generalContainer.Get(i)}");
            }

            Console.WriteLine("\n=== Демонстрация завершена успешно ===");
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}