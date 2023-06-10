using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppPackmen
{
    internal class Program
    {
        static int максимальноеЧислоЖизней = 10;
        static int жизнь = максимальноеЧислоЖизней;
        static int золото = 1;
        static string имяИгрока;
        static char иконкаИгрока = 'Я';
        static int строка = 5;
        static int столбец = 1;
        static string[] картаИзФайла;
        static char[,] картаВПамяти;
        static int поправкаНаЗаголовок = 4;
        static int строкаминотавра = 1;
        static int столбецминотавра = 10;
        static int изменениеСтрокиМинотавра = 1;

        enum Артефакты : int
        {
            проход = 0,
            стена,
            золото,
            враг,
            минотавр
        }
        static char[] наборАртефактов = {' ', '#', '$', '@', 'Z' };

        static void Main(string[] args)
        {
            НарисуйКонсоль();

            СпроситьИмяИгрока();

            НарисоватьПараметрыИгрока();

            НарисоватьКарту();

            Task.Run(() =>
            {
                do
                {
                    ХодМинотавра();
                    изменениеСтрокиМинотавра = -изменениеСтрокиМинотавра;
                }
                while (true);
            });

            Игра();

            Геймовер();
        }

        private static void ХодМинотавра()
        {
            var строкавбуфере = строка;
            var столбецвбуфере = столбец;

            Console.SetCursorPosition(столбецминотавра, строкаминотавра + поправкаНаЗаголовок);
            Console.Write(наборАртефактов[(int)Артефакты.проход]);
            картаВПамяти[строкаминотавра, столбецминотавра] = наборАртефактов[(int)Артефакты.проход];

            строкаминотавра += изменениеСтрокиМинотавра * 3;

            Console.SetCursorPosition(столбецминотавра, строкаминотавра + поправкаНаЗаголовок);
            Console.Write(наборАртефактов[(int)Артефакты.минотавр]);
            картаВПамяти[строкаминотавра, столбецминотавра] = наборАртефактов[(int)Артефакты.минотавр];

            строка = строкавбуфере;
            столбец = столбецвбуфере;
            Thread.Sleep(1000);

        }

        private static void НарисуйКонсоль()
        {
            Console.Title = "Пакмен";
            Console.Clear();
        }

        private static void Геймовер()
        {
            Console.Beep(4000, 300);
            Console.Beep(3500, 300);
            Console.Beep(3000, 300);
            Console.Beep(2500, 300);
            Console.Beep(2000, 1800);

            Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
            Console.WriteLine("G A M E    O V E R !");
            Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight - 1);
        }

        private static void Игра()
        {
            ConsoleKeyInfo нажатаяКлавиша;

            do
            {
                НарисоватьИгрока();

                нажатаяКлавиша = Console.ReadKey();

                СделатьХод(нажатаяКлавиша);
            }
            while (нажатаяКлавиша.Key != ConsoleKey.Escape && жизнь > 0);
        }

        private static void СделатьХод(ConsoleKeyInfo нажатаяКлавиша)
        {
            var строкаВБуфере = строка;
            var столбецВБуфере = столбец;

            switch (нажатаяКлавиша.Key)
            {
                case ConsoleKey.UpArrow:
                    строка -= 1;
                    break;
                case ConsoleKey.DownArrow:
                    строка += 1;
                    break;
                case ConsoleKey.LeftArrow:
                    столбец -= 1;
                    break;
                case ConsoleKey.RightArrow:
                    столбец += 1;
                    break;
            }


            if (картаИзФайла[строка - поправкаНаЗаголовок][столбец] == наборАртефактов[(int)Артефакты.стена])
            {
                //стоим на месте
                строка = строкаВБуфере;
                столбец = столбецВБуфере;
                return;
            }

            if (картаВПамяти[строка - поправкаНаЗаголовок, столбец] == наборАртефактов[(int)Артефакты.золото])
            {
                золото++;
                ВстречаСАртефактом(Артефакты.золото);
                return;
            }

            if (картаВПамяти[строка - поправкаНаЗаголовок, столбец] == наборАртефактов[(int)Артефакты.враг])
            {
                жизнь--;
                ВстречаСАртефактом(Артефакты.враг);
                return;
            }

            if (картаВПамяти[строка - поправкаНаЗаголовок, столбец] == наборАртефактов[(int)Артефакты.минотавр])
            {
                жизнь -= максимальноеЧислоЖизней;
                ВстречаСАртефактом(Артефакты.минотавр);
            }
        }

        private static void ВстречаСАртефактом(Артефакты артефакт)
        {
            картаВПамяти[строка - поправкаНаЗаголовок, столбец] = наборАртефактов[(int)Артефакты.проход];

            НарисоватьПараметрыИгрока();

            var частота = (артефакт == Артефакты.золото) ? 3000 : 2000;
            Console.Beep(частота, 200);
        }

        private static void НарисоватьИгрока()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(столбец, строка);
            Console.Write(иконкаИгрока + "\b");
        }

        private static void НарисоватьКарту()
        {
            картаИзФайла = File.ReadAllLines("карта.txt");
            картаВПамяти = new char[картаИзФайла.Length, картаИзФайла[0].Length];

            for (int x = 0; x < картаИзФайла.Length; x++)
            {
                Console.WriteLine(картаИзФайла[x]);

                for (int y = 0; y < картаИзФайла[x].Length; y++)
                    картаВПамяти[x, y] = картаИзФайла[x][y];
            }
        }

        private static void СпроситьИмяИгрока()
        {
            Console.Write("Как Вас зовут? ");
            имяИгрока = Console.ReadLine();

            Console.Clear();
            Console.Beep(2000, 300);
            Console.Beep(1500, 900);
            Console.CursorVisible = false;

            if (имяИгрока != "" && имяИгрока != " ")
            {
                иконкаИгрока = имяИгрока[0];
            }
        }

        private static void НарисоватьПараметрыИгрока()
        {
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Игрок : " + имяИгрока);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\t Нажмите ESC для выхода.");

            Console.BackgroundColor = ConsoleColor.Green;
            for (int счетчик = 0; счетчик < жизнь; счетчик++)
                Console.Write(" ");

            Console.BackgroundColor = ConsoleColor.Black;
            for (int счетчик = 0; счетчик < максимальноеЧислоЖизней - жизнь; счетчик++)
                Console.Write(" ");

            Console.WriteLine();

            Console.BackgroundColor = ConsoleColor.Yellow;
            for (int счетчик = 0; счетчик < золото; счетчик++)
                Console.Write(" ");

            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
