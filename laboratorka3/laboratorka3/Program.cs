using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Student
{
    private string _firstName;
    private string _lastName;
    private int _age;
    private double _averageGrade;

    public Student(string firstName, string lastName, int age, double averageGrade)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        AverageGrade = averageGrade;
    }

    public string FirstName
    {
        get => _firstName;
        set
        {
            ValidateName(value, nameof(FirstName));
            _firstName = value;
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            ValidateName(value, nameof(LastName));
            _lastName = value;
        }
    }

    public int Age
    {
        get => _age;
        set
        {
            if (value < 16 || value > 100)
                throw new ArgumentException("Age must be between 16 and 100");
            _age = value;
        }
    }

    public double AverageGrade
    {
        get => _averageGrade;
        set
        {
            if (value < 0 || value > 5)
                throw new ArgumentException("Average grade must be between 0 and 5");
            _averageGrade = value;
        }
    }

    private void ValidateName(string name, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{propertyName} cannot be empty");
        if (name.Length > 50)
            throw new ArgumentException($"{propertyName} must be less than 50 characters");
    }

    public override bool Equals(object obj)
    {
        return obj is Student student &&
               FirstName == student.FirstName &&
               LastName == student.LastName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FirstName, LastName);
    }
}

// 2. Класс Университет
public class University
{
    private readonly List<Student> _students = new List<Student>();

    public void AddStudent(Student student)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student), "Student cannot be null");

        if (_students.Contains(student))
            throw new InvalidOperationException("Student already exists");

        _students.Add(student);
    }

    public bool RemoveStudent(Student student)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student));

        return _students.Remove(student);
    }

    public Student FindStudent(string firstName, string lastName)
    {
        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName, nameof(lastName));

        return _students.FirstOrDefault(s =>
            s.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
            s.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<Student> GetStudents()
    {
        return _students.AsReadOnly();
    }

    private void ValidateName(string name, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{propertyName} cannot be empty");
    }
}

// 3. Пространство имен DataAccess
namespace DataAccess
{
    public interface IStudentRepository
    {
        void SaveStudents(IEnumerable<Student> students);
        List<Student> LoadStudents();
    }

    public class StudentsRepository : IStudentRepository
    {
        private readonly string _filePath;

        public StudentsRepository(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public void SaveStudents(IEnumerable<Student> students)
        {
            if (students == null)
                throw new ArgumentNullException(nameof(students));

            using var writer = new StreamWriter(_filePath);
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FirstName},{student.LastName},{student.Age},{student.AverageGrade}");
            }
        }

        public List<Student> LoadStudents()
        {
            if (!File.Exists(_filePath))
                return new List<Student>();

            var students = new List<Student>();
            using var reader = new StreamReader(_filePath);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(',');
                if (parts.Length == 4 &&
                    int.TryParse(parts[2], out int age) &&
                    double.TryParse(parts[3], out double grade))
                {
                    try
                    {
                        students.Add(new Student(parts[0], parts[1], age, grade));
                    }
                    catch (ArgumentException ex)
                    {
                        // Логирование ошибки некорректных данных
                        Console.WriteLine($"Skipped invalid student data: {ex.Message}");
                    }
                }
            }
            return students;
        }
    }
}

// Пример использования
class Program
{
    static void Main()
    {
        var university = new University();
        var repository = new DataAccess.StudentsRepository("students.txt");

        // Добавление студентов
        try
        {
            university.AddStudent(new Student("John", "Doe", 20, 4.5));
            university.AddStudent(new Student("Jane", "Smith", 22, 4.2));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding student: {ex.Message}");
        }

        // Сохранение в файл
        try
        {
            repository.SaveStudents(university.GetStudents());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving students: {ex.Message}");
        }

        // Загрузка из файла
        try
        {
            var loadedStudents = repository.LoadStudents();
            foreach (var student in loadedStudents)
            {
                Console.WriteLine($"{student.FirstName} {student.LastName}, Age: {student.Age}, Grade: {student.AverageGrade}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading students: {ex.Message}");
        }
    }
}