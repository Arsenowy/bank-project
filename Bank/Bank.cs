using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SQLite;
// im using Data.SQLite extenssion v1.0.113.7 
// by SQLite Development Team 
// website: system.data.sqlite.org


/// Dopiero w tym momencie zorientowałem się, że mogłem wszystko napisać w oparciu o ID
/// Wtedy kod można by łatwiej rozbudowywać w przyszłości
/// 

namespace Bank
{
    // ######### Database
    // ##############################################################################################
    class Database
    {
        public SQLiteConnection myConnection;
        public Database()
        {
            myConnection = new SQLiteConnection("Data Source=database.sqlite3");
            // if database doent exist -> create it
            if (!File.Exists("./database.sqlite3"))
            {
                SQLiteConnection.CreateFile("database.sqlite3");
                myConnection.Open();
                string sql = "CREATE TABLE Card (id INTEGER PRIMARY KEY, card_number TEXT, pin TEXT, balance INTEGER DEFAULT 0)";
                SQLiteCommand command = new SQLiteCommand(sql, myConnection);
                command.ExecuteNonQuery();
                myConnection.Close();
            }
        }
        public void OpenConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
            }
        }
        public void CloseConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
            }
        }
        public static bool Insert(string card, string pin, int balance = 0)
        {
            Database dbObject = new Database();

            string query = "INSERT INTO Card ('card_number', 'pin', 'balance') VALUES (@card_number, @pin, @balance)";
            SQLiteCommand insert = new SQLiteCommand(query, dbObject.myConnection);

            dbObject.OpenConnection();
            insert.Parameters.AddWithValue("@card_number", card);
            insert.Parameters.AddWithValue("@pin", pin);
            insert.Parameters.AddWithValue("@balance", balance);
            bool result = Convert.ToBoolean(insert.ExecuteNonQuery());
            dbObject.CloseConnection();

            return result;
        }
        public static bool FindCard(string cardNumber, string pin)
        {
            bool isLogin = false;
            Database dbObject = new Database();

            string query = "SELECT * FROM Card WHERE card_number = '" + cardNumber + "' LIMIT 1";
            SQLiteCommand insert = new SQLiteCommand(query, dbObject.myConnection);
            dbObject.OpenConnection();
            SQLiteDataReader result = insert.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    if (result["pin"].ToString() == pin)
                    {
                        isLogin = true;
                    }
                }
            }
            dbObject.CloseConnection();
            return isLogin;

        }
        public static int GetBalance(string card)
        {
            Database dbObject = new Database();
            int balance = 0;

            string query = "SELECT balance FROM Card WHERE card_number = '" + card + "' LIMIT 1";
            SQLiteCommand getVal = new SQLiteCommand(query, dbObject.myConnection);
            dbObject.OpenConnection();
            SQLiteDataReader result = getVal.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    balance = Convert.ToInt32(result["balance"]);
                }
            }
            dbObject.CloseConnection();
            return balance;
        }
    }
    // ######### cards
    // ##############################################################################################
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
        // ############## Account menu ##################
        public static void AccMenu(string number)
        {
            while (true)
            {
                Console.WriteLine("\nMenu");
                Console.WriteLine("1. Check balance");
                Console.WriteLine("2. Log out");
                Console.WriteLine("0. Exit");
                bool parseSuccess = Int32.TryParse(Console.ReadLine(), out int n);
                Console.Clear();

                int balance = Database.GetBalance(number);

                if (n == 1 && parseSuccess)
                {
                    Console.WriteLine("You have $" + balance);
                }
               
                else if (n == 2 && parseSuccess)
                {
                    break;
                }
                else if (n == 0 && parseSuccess) Environment.Exit(0);

                else Console.WriteLine("Bad value");
            }
        }
        
        //          ### generators ###
        ////////////////// random numbers
        private string Randimize(int n)
        {
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

    /// ########## MAIN PROGRAM ##########
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
        static void LogInto(string number, string pin)
        {
            if (Database.FindCard(number, pin))
            {
                Console.WriteLine("you have successfully logged in");
                Card.AccMenu(number);

            } else Console.WriteLine("Wrong card number or PIN!");
        }
        public static void MainMenu()
        {
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
                    Card card = new Card();
                    Database.Insert(card.CardNumber, card.Pin);

                    Console.WriteLine("Your card has been created");
                    Console.WriteLine("Your card numer: ");
                    Console.WriteLine(card.CardNumber);
                    Console.WriteLine("Your PIN numer: ");
                    Console.WriteLine(card.Pin);
                }
                else if (n == 2 && parseSuccess)
                {
                    Console.WriteLine("Enter your card number: ");
                    string number = Console.ReadLine();
                    number = number.Trim(' ');
                    //if card number is a numeric value and is of the correct length
                    if (long.TryParse(number, out _) && number.Length == 16)
                    {
                        Console.WriteLine("Enter your pin: ");
                        string pin = Console.ReadLine();
                        pin = pin.Trim(' ');
                        //if pin is a numeric value and is of the correct length
                        if (Int32.TryParse(pin, out _) && pin.Length == 4)
                        {
                            // checking the checksum of the entered number
                            if (VerifyNumber(number)) LogInto(number, pin);
                            else Console.WriteLine("Card number is incorrect");
                            // else pin is wrong
                        }
                        else Console.WriteLine("Something went wrong, try again ");
                        // else card number is wrong
                    }
                    else Console.WriteLine("Something went wrong, try again ");
                }
                else if (n == 0 && parseSuccess) Environment.Exit(0);
                else Console.WriteLine("Bad value!");
            }
        }

        static void Main()
        {
            MainMenu();
        }
    }
}
