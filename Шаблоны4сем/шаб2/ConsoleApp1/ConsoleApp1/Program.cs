using System;
using System.Collections.Generic;
using System.Linq;

// ===== Данные, проходящие через конвейер =====
public class DataContext
{
    public string Input { get; set; }
    public bool IsValid { get; set; } = true;
    public string ErrorMessage { get; set; }
    public string TransformedData { get; set; }
    public List<string> Logs { get; set; } = new List<string>();
}

// ===== Базовый обработчик (Middleware) =====
public abstract class MiddlewareHandler
{
    protected MiddlewareHandler _next;

    public void SetNext(MiddlewareHandler next)
    {
        _next = next;
    }

    public virtual void Handle(DataContext context)
    {
        // Если цепочка не прервана и есть следующий – передаём
        if (context.IsValid && _next != null)
            _next.Handle(context);
    }
}

// ===== 1. Валидация: проверяем, что данные не пустые =====
public class ValidationMiddleware : MiddlewareHandler
{
    public override void Handle(DataContext context)
    {
        Console.WriteLine("[Validation] Проверка данных...");

        if (string.IsNullOrWhiteSpace(context.Input))
        {
            context.IsValid = false;
            context.ErrorMessage = "Входные данные не могут быть пустыми.";
            Console.WriteLine("[Validation] ОШИБКА: данные пусты.");
            return; // прерываем цепочку
        }

        if (context.Input.Length < 3)
        {
            context.IsValid = false;
            context.ErrorMessage = "Данные должны содержать минимум 3 символа.";
            Console.WriteLine("[Validation] ОШИБКА: слишком короткие данные.");
            return;
        }

        Console.WriteLine("[Validation] Данные валидны.");
        base.Handle(context); // передаём дальше
    }
}

// ===== 2. Преобразование: приводим к верхнему регистру =====
public class TransformationMiddleware : MiddlewareHandler
{
    public override void Handle(DataContext context)
    {
        Console.WriteLine("[Transformation] Преобразование в верхний регистр...");
        context.TransformedData = context.Input.ToUpperInvariant();
        Console.WriteLine($"[Transformation] Результат: {context.TransformedData}");

        base.Handle(context);
    }
}

// ===== 3. Логирование: записываем действие в лог =====
public class LoggingMiddleware : MiddlewareHandler
{
    public override void Handle(DataContext context)
    {
        Console.WriteLine("[Logging] Сохранение лога...");
        string logEntry = $"{DateTime.Now:HH:mm:ss} | Исходные: {context.Input} -> Преобразованные: {context.TransformedData}";
        context.Logs.Add(logEntry);
        Console.WriteLine($"[Logging] Лог добавлен: {logEntry}");

        base.Handle(context);
    }
}

// ===== Конвейер для удобного управления =====
public class Pipeline
{
    private readonly List<MiddlewareHandler> _handlers = new List<MiddlewareHandler>();

    public Pipeline Use<T>() where T : MiddlewareHandler, new()
    {
        var handler = new T();
        _handlers.Add(handler);
        return this;
    }

    public void Execute(DataContext context)
    {
        if (_handlers.Count == 0) return;

        // Строим цепочку
        for (int i = 0; i < _handlers.Count - 1; i++)
        {
            _handlers[i].SetNext(_handlers[i + 1]);
        }

        // Запускаем с первого
        _handlers[0].Handle(context);
    }
}

// ===== Демонстрация =====
class Program
{
    static void Main()
    {
        Console.WriteLine("=== Middleware Pipeline Demo ===\n");

        // Пример 1: успешная обработка
        var context1 = new DataContext { Input = "  hello world  " };
        RunPipeline(context1);

        Console.WriteLine("\n---\n");

        // Пример 2: ошибка валидации
        var context2 = new DataContext { Input = "a" };
        RunPipeline(context2);

        Console.WriteLine("\n---\n");

        // Пример 3: пустые данные
        var context3 = new DataContext { Input = "" };
        RunPipeline(context3);
    }

    static void RunPipeline(DataContext ctx)
    {
        var pipeline = new Pipeline();
        pipeline
            .Use<ValidationMiddleware>()
            .Use<TransformationMiddleware>()
            .Use<LoggingMiddleware>();

        pipeline.Execute(ctx);

        Console.WriteLine($"\nРезультат обработки: Валидно = {ctx.IsValid}");
        if (!ctx.IsValid)
            Console.WriteLine($"Ошибка: {ctx.ErrorMessage}");
        else
            Console.WriteLine($"Финальные данные: {ctx.TransformedData}");

        if (ctx.Logs.Any())
            Console.WriteLine($"Логов собрано: {ctx.Logs.Count}");
    }
}