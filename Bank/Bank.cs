using System;
using System.Collections.Generic;
using SQLite;

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
            CheckSum = this.GenerateChecksum();
            CardNumber = this.Iin + this.AccID + this.CheckSum;
            Pin = this.Randimize(4);
            AccBalance = 0;
            Counter++;
        }
        // Account menu
        public void AccMenu()
        {
            while (true)
            {
                Console.WriteLine("\nMenu");
                Console.WriteLine("1. Check balance");
                Console.WriteLine("2. Log out");
                Console.WriteLine("0. Exit");
                bool parseSuccess = Int32.TryParse(Console.ReadLine(), out int n);
                Console.Clear();

                if (n == 1 && parseSuccess)
                {
                    this.AccountInfo();
                }
                else if (n == 2 && parseSuccess)
                {
                    break;
                }
                else if (n == 0 && parseSuccess) Environment.Exit(0);

                else Console.WriteLine("Zła wartość");
            }
        }

        public void AccountInfo()
        {
            Console.WriteLine("Your account balance is $" + this.AccBalance);
        }
        
        //          ### generators ###
        ////////////////// random numbers
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

        ////////////////// checksum generator
        public string GenerateChecksum()
        {
            string num = this.Iin + this.AccID;
            return LuhnGen(num);
        }
        public static string LuhnGen(string num)
        {
            int len = num.Length;
            float sum = 0;
            int factor, i = 0;

            while (len > 0)
            {
                if (len % 2 == 0)
                {
                    factor = 1;
                }
                else factor = 2;

                // get int value of char on position i and multiply it
                int temp = Convert.ToInt32(Char.GetNumericValue(num, i)) * factor;
                if (temp >= 10)
                {
                    // temp = 1 + (temp - 10) -> temp = temp - 9
                    // ex. 13  |  1 + (13 - 10) = 4
                    temp -= 9;
                }
                sum += temp;
                len--;
                i++;
            }
            sum = sum % 10;
            if (sum != 0) sum = 10 - sum;

            return sum.ToString();
        }
        
        //              ### class variables ###
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
        // counter which is used for count accounts in list
        // total number of accounts created
        public static int Counter = 0;
    }

    class Bank
    {
        static bool VerifyNumber(string number)
        {
            // slice number and checksum
            string temp = number.Substring(0, number.Length-1);
            string checksum = Card.LuhnGen(temp);
            temp = number.Substring(number.Length-1);
            if (checksum == temp) return true;
            else return false;
        }
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
                bool parseSuccess = Int32.TryParse(Console.ReadLine(), out int n);
                Console.Clear();

                if (n == 1 && parseSuccess)
                {
                    cards.Add(new Card());
                    Console.WriteLine("Your card has been created");
                    Console.WriteLine("Your card numer: ");
                    Console.WriteLine(cards[Card.Counter - 1].CardNumber);

                    Console.WriteLine("Your PIN numer: ");
                    Console.WriteLine(cards[Card.Counter - 1].Pin);
                }
                else if (n == 2 && parseSuccess)
                {
                    Console.WriteLine("Enter your card number: ");
                    string number = Console.ReadLine();
                    number = number.Trim(' ');

                    Console.WriteLine("Enter your pin: ");
                    string pin = Console.ReadLine();
                    pin = pin.Trim(' ');

                    // checking the checksum of the entered number
                    if (VerifyNumber(number)) LogInto(cards, number, pin);
                    else Console.WriteLine("Card number is incorrect");

                }
                else if (n == 0 && parseSuccess) Environment.Exit(0);
                else Console.WriteLine("Zła wartość");
            }
        }

        static void Main()
        {
            
            MainMenu();
        }
    }
}
