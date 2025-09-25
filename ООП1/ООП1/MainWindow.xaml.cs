using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ООП1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnCreatePerson_Click(object sender, RoutedEventArgs e)
        {
            Person person = new Person("Ben", 18, "ben@gmail.com");

            txtName.Text = person.name;
            txtAge.Text = person.age.ToString();
            txtEmail.Text = person.email;
        }
        class Person
        {
            public string name = "Ben";
            public int age = 18;
            public string email = "ben@gmail.com";

            public Person(string name)
            {
                this.name = name;
            }

            public Person(string name, int age) : this(name)
            {
                this.age = age;
            }

            public Person(string name, int age, string email) : this("Bob", age)
            {
                this.email = email;
            }
        
        }
    }
}
