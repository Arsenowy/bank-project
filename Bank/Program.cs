using System;
using System.Collections.Generic;

namespace Bank
{
    interface ICards
    {
        string Mii { get; }
        string Iin { get; }
        string AccID { get; }
        string CheckSum { get; }
        string CardNumber { get; }
        string Pin { get; }
        int AccBalance { get; set; }

    }

    class Card : ICards
    {
        //constructor
        public Card()
        {
            Mii = "4";
            // concat strings 4 and 00000
            Iin = this.Mii.ToString() + "00000";
            AccID = this.Randimize(9);
            CheckSum = "7";   // change it later
            CardNumber = this.Iin + this.AccID + this.CheckSum;
            Pin = this.Randimize(4);
            AccBalance = 0;
            Counter++;
        }
        public void AccMenu()
        {
            while (true)
            {
                Console.WriteLine("\nMenu");
                Console.WriteLine("1. Check balance");
                Console.WriteLine("0. Exit");
                int n = int.Parse(Console.ReadLine());
                Console.Clear();

                if (n == 1)
                {
                    this.AccountInfo();
                }
                else if (n == 2)
                {

                }
                else if (n == 0) break;
                else Console.WriteLine("Zła wartość");
            }
        }
        public void AccountInfo()
        {
            Console.WriteLine("Your account balance is $" + this.AccBalance);
        }

        private string Randimize(int n)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // na samym początku początku moim pomysłem było napisanie własnej funkcji
            // służącej do dopisywania zer w przypadku wylosowania mniejszej liczby niż
            // 4-ro lub 9-cio cyfrowej
            // jednak myśląc nad tym zagadnieniem z Wojtkiem Kędzierskim 
            // została znaleziona do tego dedykowana funkcja w c# - wpisanie D* w argumencie funkcji tostring,
            //więc zostawiam tylko moją metodę w komentarzu
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //Random r = new Random();
            //int rInt = 0;
            ////a random number with 4 digits
            //if (n == 4) rInt = r.Next(0, 9999);
            //else if (n == 9) rInt = r.Next(0, 999999999);
            //else
            //{
            //    Console.WriteLine("Enter proper value of length");
            //    return null;
            //}
            //string random = rInt.ToString();
            //string zero = "";
            //for (int i = 0; i < n - random.Length; i++)
            //{
            //    zero += "0";
            //}
            //return zero + random;
            ///////////////////////////////////////////

            Random r = new Random();
            int rInt;

            if (n == 4)
            {
                rInt = r.Next(0, 9999);
                return rInt.ToString("D4");
            }
            else if (n == 9) { 
                rInt = r.Next(0, 999999999);
                return rInt.ToString("D9");
            }
            else
            {
                Console.WriteLine("Enter proper value of length");
                return null;
            }
        }
        

        // first part of card number - what sort of institution issued the card
        public string Mii { get; }
        // second part of card number - name of bank 
        public string Iin { get; }
        // third part of card number - unique random 9-digit number
        public string AccID { get; }
        // four part of card number - checksum
        public string CheckSum { get; }
        public string CardNumber { get; }
        public string Pin { get; }
        public int AccBalance { get; set; }
        public static int Counter = 0;
    }

    class Program
    {
        static void LogInto(List<Card> accounts, string number, string pin)
        {
            bool isLogIn = false;
            foreach (Card item in accounts)
            {
                if (item.CardNumber == number && item.Pin == pin)
                {
                    isLogIn = true;
                    Console.WriteLine("You have successfully logged in!");
                    item.AccMenu();
                    break;
                }
            }
            if (isLogIn == false)
            {
                Console.WriteLine("Wrong card number or PIN!");
            }
        }
        public static void MainMenu()
        {
            List<Card> cards = new List<Card>();

            while (true)
            {
                Console.WriteLine("\nMenu");
                Console.WriteLine("1. Create an account");
                Console.WriteLine("2. Log into account");
                Console.WriteLine("0. Exit");
                int n = int.Parse(Console.ReadLine());
                Console.Clear();

                if (n == 1)
                {
                    cards.Add(new Card());
                    Console.WriteLine("Your card has been created");
                    Console.WriteLine("Your card numer: ");
                    Console.WriteLine(cards[Card.Counter - 1].CardNumber);

                    Console.WriteLine("Your PIN numer: ");
                    Console.WriteLine(cards[Card.Counter - 1].Pin);
                }
                else if (n == 2)
                {
                    Console.WriteLine("Enter your card number: ");
                    string number = Console.ReadLine();

                    Console.WriteLine("Enter your card number: ");
                    string pin = Console.ReadLine();

                    LogInto(cards, number, pin);
                }
                else if (n == 0) break;
                else Console.WriteLine("Zła wartość");
            }
        }

        static void Main()
        {
            MainMenu();
        }
    }
}
