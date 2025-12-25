using System;

namespace FootballTeams
{
    public abstract class FootballTeam
    {
        public abstract string Name { get; }
        public abstract string City { get; }
        public abstract int Trophies { get; }

        public abstract void Play();
        public abstract void Train();
    }

    public class CSKA : FootballTeam
    {
        public override string Name => "ЦСКА";
        public override string City => "Москва";
        public override int Trophies => 13;

        public bool IsArmyTeam { get; } = true;

        public override void Play()
        {
            Console.WriteLine($"{Name}: Играем в армейском стиле!");
        }

        public override void Train()
        {
            Console.WriteLine($"{Name}: Тренируемся на базе в Новогорске");
        }

        public void ShowArmyTradition()
        {
            Console.WriteLine($"{Name}: Армейские традиции с 1911 года!");
        }
    }

    public class Spartak : FootballTeam
    {
        public override string Name => "Спартак";
        public override string City => "Москва";
        public override int Trophies => 12;

        public string MainColors { get; } = "Красно-белые";

        public override void Play()
        {
            Console.WriteLine($"{Name}: Атакуем под песни болельщиков!");
        }

        public override void Train()
        {
            Console.WriteLine($"{Name}: Тренируемся в Тушино");
        }

        public void ChantFansSong()
        {
            Console.WriteLine($"{Name}: Спар-так! Спар-так! Вперёд!");
        }
    }

    public class Zenit : FootballTeam
    {
        public override string Name => "Зенит";
        public override string City => "Санкт-Петербург";
        public override int Trophies => 14;

        public string StadiumSponsor { get; } = "Газпром Арена";

        public override void Play()
        {
            Console.WriteLine($"{Name}: Играем в северной столице!");
        }

        public override void Train()
        {
            Console.WriteLine($"{Name}: Тренируемся в Удельном парке");
        }

        public void ShowEuroCup()
        {
            Console.WriteLine($"{Name}: Обладатель Кубка УЕФА 2008!");
        }
    }

    class Program
    {
        static void ShowTeamInfo(FootballTeam team)
        {
            Console.WriteLine($"Команда: {team.Name}");
            Console.WriteLine($"Город: {team.City}");
            Console.WriteLine($"Трофеев: {team.Trophies}");

            if (team is CSKA cska)
            {
                Console.WriteLine($"Армейский клуб: {cska.IsArmyTeam}");
                cska.ShowArmyTradition();
            }
            else if (team is Spartak spartak)
            {
                Console.WriteLine($"Основные цвета: {spartak.MainColors}");
                spartak.ChantFansSong();
            }
            else if (team is Zenit zenit)
            {
                Console.WriteLine($"Стадион: {zenit.StadiumSponsor}");
                zenit.ShowEuroCup();
            }

            team.Train();
            team.Play();
        }

        static void Main()
        {
            Console.WriteLine("Футбольные команды России:\n");

            ShowTeamInfo(new CSKA());
            ShowTeamInfo(new Spartak());
            ShowTeamInfo(new Zenit());

            Console.WriteLine("\n\nДополнительная информация:");

            CSKA cskaTeam = new CSKA();
            Console.WriteLine($"\nCSKA - армейская команда: {cskaTeam.IsArmyTeam}");
            cskaTeam.ShowArmyTradition();

            Spartak spartakTeam = new Spartak();
            Console.WriteLine($"\nСпартак - цвета: {spartakTeam.MainColors}");
            spartakTeam.ChantFansSong();

            Zenit zenitTeam = new Zenit();
            Console.WriteLine($"\nЗенит - домашний стадион: {zenitTeam.StadiumSponsor}");
            zenitTeam.ShowEuroCup();
        }
    }
}