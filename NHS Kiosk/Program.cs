using System;
using System.Diagnostics;

namespace NoHomoSapiens
{
    class Program
    {
        //Declaring our Global Bank Struct
        struct Bank
        {
            public int[] CurrencyAmount;
            public string[] CurrencyName;
            public decimal[] CurrencyValue;
            public int[] UserPayment;
        }
        //End of Global Bank Struct

        //Creating a new struct for Transaction
        public struct Transaction
        {
            public  int transactionNum;
            public  string transactionDate;
            public  decimal cashAmount;
            public  string cardVendor;
            public  decimal cardAmount;
            public  decimal changeGiven;
        }

        public static Transaction trans;

        static void Main(string[] args)
        {
            //card test value = 4452720412345678   Visa
            //card test value = 3792720412345679   American Express
            //card test value = 6552720412365878   Discover
            //card test value = 5152720412345678   MasterCard

            //Declaring variables and instance of Bank struct
            decimal changeOwed = 0;
            Bank kiosk;

            DateTime dtmTime;
            dtmTime = DateTime.Now;

            //Declaring arrays
            kiosk.CurrencyName = new string[] { "Hundreds", "Fifties", "Twenties", "Tens", "Fives", "Twos",
                                                "Ones", "Half Dollar", "Quarters", "Dimes", "Nickels", "Pennies" };
            kiosk.CurrencyAmount = new int[12];
            kiosk.CurrencyValue = new decimal[] { 100, 50, 20, 10, 5, 2, 1, .50m, .25m, .10m, .05m, .01m };
            kiosk.UserPayment = new int[12];

            //Setting Variables
            trans.transactionNum = 1;
            trans.transactionDate = dtmTime.ToString("MMM-dd-yyyy,HH:mm");
            trans.cashAmount = 0;
            trans.cardVendor = "No Vendor Given";
            trans.cardAmount = 0;
            trans.changeGiven = 0;


            //Filling the "Bank" with bills/coins
            for (int index = 0; index < kiosk.CurrencyAmount.Length; index++)
            {
                kiosk.CurrencyAmount[index] = 5;
            }

            //Loop that will run the kiosk forever.
            while (true)
            {
                //Displaying a welcome message and how to use the kiosk
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("WELCOME! This is the NoHomoSapiens self-checkout kiosk!");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Please enter your item prices now and press ENTER when you are done.");
                Console.WriteLine();

                //Running function to input prices
                decimal totalPrice = InputItemPrices();

                //Displaying the total
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Total\t\t$" + totalPrice);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();


                //Calling function to see if the user is using a card 
                //and if so the function contains function calls for the card
                totalPrice = UsingCard(kiosk, totalPrice);
                Console.WriteLine();

                //Declaring new variables to fix a decimal place bug
                string decimalString = totalPrice.ToString();
                int decimalIndex = 0;

                //for loop to find the "." in the string version of the totalPrice
                for (int index = 0; index < decimalString.Length; index++)
                {
                    if (decimalString[index] == '.') decimalIndex = index; //Setting decimalIndex to the place of the "."
                }

                if (totalPrice > 0 && decimalString.Length > 3)
                {
                    //Making sure there are no numbers more than 2 places after the "."
                    decimalString = decimalString.Substring(0, decimalIndex + 3);
                }

                //reverting the string back into a decimal
                totalPrice = decimal.Parse(decimalString);

                //if the total price is still greater than 0 and the user is not using a card
                //the user will be prompted to pay the remainder in cash
                if (totalPrice > 0)
                {

                    //Running function to input payment
                    changeOwed = CashPayment(kiosk, totalPrice);
                    Console.WriteLine();

                    //showing the amount of change
                    Console.WriteLine("Change {0:C}", changeOwed);
                   
                    //if the kiosk owes the user change this section will run.
                    //and see if the bank has enough change to dispense. 
                    //If the bank does not have enough cash for change the user is refunded.
                    if (changeOwed > 0)
                    {
                        Console.WriteLine();
                        //running function to see if the bank has enough change
                        bool canPay = CheckBank(changeOwed, kiosk);

                        //running function to dispense change
                        DispenseChange(changeOwed, kiosk, canPay);
                    }

                    //Message to User
                    Console.WriteLine();
                    Console.WriteLine("Thank you for using the NoHomoSapiens self-checkout kiosk!");
                    Console.WriteLine("Have a great day!");
                }

                Logging();
                trans.transactionNum += 1;

                //Call Kiosk Transaction Reset Function to reset the kiosk for the next transaction
                KioskTransactionReset(kiosk);
            }
        }//End of Main

        #region InputOfItemPrices
        static decimal InputItemPrices() //Start of Input Item Prices
        {
            //Declaring variables
            string inputItem = "";
            int itemCount = 1;
            decimal total = 0;
            bool realAmount = true;
            int checker = 0;

            //Do while loop for input validation
            do
            {
                //User enters an item price
                Console.Write("Item {0}\t\t$", itemCount);
                inputItem = Console.ReadLine();

                //Check to make sure the input does not have more than 2 decimal points
                checker = 0;
                for (int index = 0; index < inputItem.Length; index++)
                {
                    if (inputItem[index] == '.')
                    {
                        for (int count = index + 1; count < inputItem.Length; count++)
                        {
                            checker++;
                        }
                    }
                }

                //setting varibale to T/F depending if the value is real or not
                if (checker > 2) realAmount = false;
                else realAmount = true;


                //Checking to see if the user inputed a real price that is not negative
                if (decimal.TryParse(inputItem, out _) && decimal.Parse(inputItem) > 0 && realAmount == true)
                {
                    //adding the item price to the total price
                    total = total + decimal.Parse(inputItem);
                    itemCount++;
                }
                else if (inputItem != "") Console.WriteLine("Enter a valid item price.");

            } while (inputItem != "");

            //Returning the total price
            return total;

        }//End of Input Item Prices
        #endregion

        #region CashPayment
        static decimal CashPayment(Bank bank, decimal total) //Start of Cash Payment
        {
            //Declaring Variables
            int paymentCount = 1;
            string payment = "";
            bool goodPayment = false;

            //Message to user
            Console.WriteLine("You are paying with cash.");
            Console.WriteLine("Insert current US bills or coins now.");
            Console.WriteLine();
            
            //Do while loop for validation
            do
            {
                //Getting user input for the payment in cash
                Console.Write("Payment {0}\t$", paymentCount);
                payment = Console.ReadLine();
                

                //Validating the payment input to make sure it's a positive number
                if (decimal.TryParse(payment, out _) && decimal.Parse(payment) > 0)
                {

                    //loop through the array of values to test if the payment is a real bill or coin
                    for (int index = 0; index < bank.CurrencyValue.Length; index++)
                    {
                        //checking if the payment is a valid bill/coin
                        if (decimal.Parse(payment) == bank.CurrencyValue[index])
                        {
                            goodPayment = true;
                            bank.UserPayment[index]++;
                            bank.CurrencyAmount[index]++;
                            break;
                        }
                        else if (decimal.Parse(payment) < 0.01m && decimal.Parse(payment) > 0)
                        {
                            goodPayment = true;
                            bank.UserPayment[11]++;
                            bank.CurrencyAmount[11]++;
                            break;
                        }
                        else goodPayment = false;
                    }
                }

                //if the payment is valid then it is subtracted from the total
                if (goodPayment == true)
                {
                    trans.cashAmount += decimal.Parse(payment);
                    total = total - decimal.Parse(payment);
                    paymentCount++;
                }

                //Tells the user their payment is invalid
                else Console.WriteLine("Enter a valid bill or coin.");

                //Displays the remaining value if there is any
                if (total > 0) Console.WriteLine("Remaining\t{0:C}", total);
                Console.WriteLine();

            } while (total > 0);

            //returns what change needs to be provided
            return total * -1;

        }//end of Cash Payment
        #endregion

        #region CheckBank
        static bool CheckBank(decimal changeOwed, Bank bank) //Start of Check Bank
        {
            //Declaring Variable and Array
            decimal owed = changeOwed;
            int[] array = new int[12];

            //Filling the new array with the amount of bills and coins
            for (int index = 0; index < bank.CurrencyAmount.Length; index++)
            {
                array[index] = bank.CurrencyAmount[index];
            }

            //Loop to see if the bank has enough money for the transaction
            int newIndex = 0;
            while (owed > 0)
            {
                //if statement that checks which bills can be used and if that bill is available
                if (owed >= bank.CurrencyValue[newIndex] && array[newIndex] > 0)
                {
                    array[newIndex]--;
                    owed -= bank.CurrencyValue[newIndex];
                    newIndex = 0;
                }
                //If the kiosk does not have enough change
                else if (owed > 0 && array[newIndex] == 0)
                {
                    //Displays message to user
                    Console.WriteLine("The kiosk does not have enough available currency to dispense change.");
                    Console.WriteLine();

                    //Loop to refund the user's money
                    for (int count = 0; count < bank.UserPayment.Length; count++)
                    {
                        if (bank.UserPayment[count] > 0)
                        {
                            //Money is refunded and taken back out of the bank
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("{0:C} dispensed", bank.CurrencyValue[count]);
                            Console.ForegroundColor = ConsoleColor.White;
                            bank.UserPayment[count]--;
                            bank.CurrencyAmount[count]--;
                            count = 0;
                        }
                    }
                    Console.WriteLine("");

                    //Validation loop for paying with a card
                    string response = "";
                    do
                    {
                        Console.Write("Would you like to pay using a card? (y/n) ");
                        response = Console.ReadLine();

                        if (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n")
                        {
                            Console.WriteLine("Please enter (y/n) ");
                        }
                    } while (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n");

                    //If paying with a card, run the UsingCard function
                    if (response == "y")
                    {
                        UsingCard(bank, owed);
                    }
                    break;
                }
                newIndex++;
            }

            //If the amount owed is still greater than zero then function returns false
            if (owed > 0) return false;

            //return true if the bank has enough money
            return true;
        }//End of Check Bank
        #endregion

        #region DispenseChange
        static void DispenseChange(decimal changeOwed, Bank bank, bool canPay) //Start of Dispense Change
        {
            //Will only run if the bank has enough money to pay
            if (canPay == true)
            {
                //Loop that will run until the change has been dispensed
                int index = 0;
                trans.changeGiven = changeOwed;
                while (changeOwed > 0)
                {
                    //Checks to see ig the bank can pay in a certain bill/coin
                    if (changeOwed >= bank.CurrencyValue[index] && bank.CurrencyAmount[index] > 0)
                    {
                        //Dispenses bill/coin
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("{0:C} dispensed", bank.CurrencyValue[index]);
                        Console.ForegroundColor = ConsoleColor.White;
                        //Takes bill/coin out of the bank
                        bank.CurrencyAmount[index]--;
                        //Subtracts amount from change owed
                        changeOwed -= bank.CurrencyValue[index];
                        //starts back at the top of the list of bills/coins
                        index = 0;
                    }
                    index++;
                }
            }
        }//End of Dispense Change
        #endregion

        #region CardNameCheck
        static string CardNameCheck(string cardNum) //Start of Card Name Check
        {
            char[] chrArray = cardNum.ToCharArray();

            //Identify the name of the card vendor
            if ((chrArray[0] == '3' && chrArray[1] == '4') || (chrArray[0] == '3' && chrArray[1] == '7'))
                return "American Express";
            else if ((chrArray[0] == '6' && chrArray[1] == '0') || (chrArray[0] == '6' && chrArray[1] == '5'))
                return "Discover";
            else if ((chrArray[0] == '5' && ((int)chrArray[1] < '6' && (int)chrArray[1] > '0')))
                return "MasterCard";
            else if (chrArray[0] == '4')
                return "Visa";
            else return "INVALID CARD NAME";
        } //End of Card Name Check
        #endregion

        #region LuhnCheck
        static bool LuhnCheck(string cardNum) //Start of Luhn Check
        {
            //Declaring variables
            int[] array = new int[cardNum.Length];
            bool isDouble = true;
            char[] chrArray = cardNum.ToCharArray();
            int sum = 0;
            int doubled = 0;
            string arrayConvert;

            //Loop that takes the char array and converts to an int array
            for (int index = 0; index < cardNum.Length; index++)
            {
                arrayConvert = chrArray[index].ToString();

                array[index] = int.Parse(arrayConvert);
            }

            /*
            Loop to run Luhn Algorithm. Starting from the rightmost digit,
            every other digit is doubled. If a doubled digit is > 9
            then 9 is subtracted from the doubled digit and then added to the sum. 
            The digits that were not doubled are immediately added to the sum.
            If the sum is evenly divisible by 10 then the card has passed the Luhn Algorithm.
            */
            for (int index = cardNum.Length - 2; index >= 0; index--)
            {
                //Checks and runs on every other number
                if (isDouble)
                {
                    //doubles
                    doubled = array[index] * 2;

                    //if > 9, 9 is subtracted then number is added to sum
                    if (doubled > 9)
                    {
                        sum += doubled - 9;
                    }
                    //if < 9 number is added to sum
                    else sum += doubled;
                }
                //if not a number that gets doubled it is added to sum
                else sum += array[index];

                //changes to false on every other number
                isDouble = !isDouble;
            }

            //adding the far right number to the sum
            sum += array[15];

            //if eveny divisible by 10, returns true. if not returns false
            return (sum % 10 == 0);

        } //End of Luhn Check
        #endregion

        #region CashBack
        static void CashBack(Bank bank) //Start of CashBack 
        {
            //Declaring Variables
            string response = "";
            int checker = 0;
            bool realAmount = false;

            //Validation loop for receive cashback option
            do
            {
                Console.Write("Would you like to recieve cash back? (y/n) ");
                response = Console.ReadLine();

                if (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n")
                {
                    Console.WriteLine("Please enter (y/n) ");
                }
            } while (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n");

            //If they want cashback
            if (response.ToLower().Substring(0, 1) == "y")
            {
                bool validAmount = false;
                bool canPay = false;

                //Validation loop for valid amount of cashback
                do
                {
                    //Message to User
                    Console.Write("How much cash back would you like to recieve? $");
                    string amount = Console.ReadLine();

                    checker = 0;

                    //Loop to make sure there are no more than 2 decimal places
                    for (int index = 0; index < amount.Length; index++)
                    {
                        if (amount[index] == '.')
                        {

                            for (int count = index + 1; count < amount.Length; count++)
                            {
                                checker++;
                            }
                        }
                    }

                    if (checker > 2) realAmount = false;
                    else realAmount = true;


                    //Checking to see if the user inputed a real number that is not negative
                    if (decimal.TryParse(amount, out _) && decimal.Parse(amount) > 0 && realAmount)
                    {
                        //Seeing if the bank has enough money to dispense cashback 
                        canPay = CheckCashBack(decimal.Parse(amount), bank);
                        Console.WriteLine();
                        Console.WriteLine("_______________________________________________");
                        Console.WriteLine();

                        //attempting to dispense cashback
                        DispenseChange(decimal.Parse(amount), bank, canPay);

                        validAmount = true;
                    }
                    else Console.WriteLine("That is not a valid amount.");

                } while (validAmount == false || canPay == false);
            }
        } //End of CashBack 
        #endregion

        #region CheckCashBack
        static bool CheckCashBack(decimal amount, Bank bank) //Start of Check Cash Back
        {
            //Declaring Variables
            decimal owed = amount;
            int[] array = new int[12];

            //Filling the new array with the amount of bills and coins
            for (int index = 0; index < bank.CurrencyAmount.Length; index++)
            {
                array[index] = bank.CurrencyAmount[index];
            }

            //Loop to see if the bank has enough money for the transaction
            for (int index = 0; owed > 0; index++)
            {
                if (owed >= bank.CurrencyValue[index] && array[index] > 0)
                {
                    array[index]--;
                    owed -= bank.CurrencyValue[index];
                    index = 0;
                }
                //If the amount owed is still greater than 0 and the bank does not have enough funds this will run
                else if (owed > 0 && array[index] == 0)
                {
                    Console.WriteLine("The kiosk does not have enough available currency to dispense that amount of cash back.");
                    break;
                }
            }

            //If the amount owed is still > 0 the function returns false
            if (owed > 0) return false;

            //return true if the bank has enough money
            return true;

        }//End of Check Cash Back
        #endregion

        #region MoneyRequest
        static string[] MoneyRequest(string account_number, decimal amount) //Start of Money Request
        {
            Random rnd = new Random();
            //50% CHANCE TRANSACTION PASSES OR FAILS
            bool pass = rnd.Next(100) < 50;
            //50% CHANCE THAT A FAILED TRANSACTION IS DECLINED
            bool declined = rnd.Next(100) < 50;
            if (pass)
            {
                return new string[] { account_number, amount.ToString() };
            }
            else
            {
                if (!declined)
                {
                    return new string[] { account_number, (amount / rnd.Next(2, 6)).ToString() };
                }
                else
                {
                    return new string[] { account_number, "declined" };
                }
            }
        }//End of Money Request
        #endregion

        #region UsingCard
        static decimal UsingCard(Bank bank, decimal amount) //Start of Using Card
        {
            //Declaring Variables
            string response = "";
            string cardNum = "";

            //Validation loop for using a card
            do
            {
                Console.Write("Would you like to pay using a card? (y/n) ");
                response = Console.ReadLine();

                if (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n")
                {
                    Console.WriteLine("Please enter (y/n) ");
                }
            } while (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n");

            //If using a card
            if (response.ToLower().Substring(0, 1) == "y")
            {
                //Validation loop for entering card number
                do
                {
                    Console.WriteLine();
                    Console.Write("Enter your card number: ");
                    cardNum = Console.ReadLine();

                    //Simulate card read error
                    while (cardNum.Length != 16)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Card Read Error");
                        Console.WriteLine("Please swipe your card again");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Enter your card number: ");
                        cardNum = Console.ReadLine();
                    }
                    Console.WriteLine();

                    //Checking the Card Vendor Name
                    string cardName = CardNameCheck(cardNum);
                    trans.cardVendor = cardName;

                    //Tells the user what kind of card it is
                    Console.WriteLine("You are using a {0}", cardName);

                    //Validating the vandor name is an accepted vendor
                    if (cardName == "Visa" || cardName == "Discover" || cardName == "American Express" || cardName == "MasterCard")
                    {
                        //Running the luhn check on the card
                        bool validCard = LuhnCheck(cardNum);

                        //Card passed Luhn Check
                        if (validCard)
                        {
                            //Running money request function
                            string[] cardPayment = MoneyRequest(cardNum, amount);

                            //If card was NOT declined
                            if (cardPayment[1] != "declined")
                            {
                                //Subtracts the payment from the total
                                amount -= decimal.Parse(cardPayment[1]);
                                trans.cardAmount += decimal.Parse(cardPayment[1]);

                                //If total is still greater than 0
                                if (amount > 0)
                                {
                                    //Message to User
                                    Console.WriteLine("Your card does not have enough funds to pay the total amount.");
                                    Console.WriteLine("{0:C} Remaining to pay", amount);
                                    Console.WriteLine();

                                    //Validating asking the user to use another card
                                    do
                                    {
                                        Console.Write("Would you like to try another card? (y/n) ");
                                        response = Console.ReadLine();

                                        if (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n")
                                        {
                                            Console.WriteLine("Please enter (y/n) ");
                                        }
                                    } while (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n");

                                    //Stops the UsingCard function if user answer is not yes
                                    if (response.ToLower().Substring(0, 1) != "y") break;
                                }
                            }
                            //Card was declined
                            else
                            {
                                Console.WriteLine("Your card has been declined.");
                                Console.WriteLine();
                                //Validating asking the user to use another card
                                do
                                {
                                    Console.Write("Would you like to try another card? (y/n) ");
                                    response = Console.ReadLine();

                                    if (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n")
                                    {
                                        Console.WriteLine("Please enter (y/n) ");
                                    }
                                } while (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n");

                                //Stops the UsingCard function if user answer is not yes
                                if (response.ToLower().Substring(0, 1) != "y") break;
                            }
                        }
                        //Card did NOT pass Luhn Check
                        else
                        {
                            Console.WriteLine("That is not a valid card.");
                            Console.WriteLine();
                            //Validating asking the user to use another card
                            do
                            {
                                Console.Write("Would you like to try another card? (y/n) ");
                                response = Console.ReadLine();

                                if (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n")
                                {
                                    Console.WriteLine("Please enter (y/n) ");
                                }

                            } while (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n");

                            //Stops the UsingCard function if user answer is not yes
                            if (response.ToLower().Substring(0, 1) != "y") break;
                        }
                    }

                    //The card did not pass the card name check
                    else
                    {
                        Console.WriteLine("That is not a valid card.");
                        Console.WriteLine();
                        //Validating asking the user to use another card
                        do
                        {
                            Console.Write("Would you like to try another card? (y/n) ");
                            response = Console.ReadLine();

                            if (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n")
                            {
                                Console.WriteLine("Please enter (y/n) ");
                            }
                        } while (response.ToLower().Substring(0, 1) != "y" && response.ToLower().Substring(0, 1) != "n");

                        //Stops the UsingCard function if user answer is not yes
                        if (response.ToLower().Substring(0, 1) != "y") break;
                    }
                    //Loop ends naturally when total is less than 0
                } while (amount > 0);
            }

            //If the full amount was paid
            if (amount == 0)
            {
                Console.WriteLine("Your payment has been confirmed.");
                Console.WriteLine();

                //Running Cash Back Function
                CashBack(bank);
            }

            //Returns total even if there is nothing left to pay
            return amount;

        }//End of Using Card
        #endregion

        #region TransactionReset
        static void KioskTransactionReset(Bank kiosk) //Start of Transaction Reset
        {
            Console.WriteLine();

            //Display bank funds after transaction
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Amount of each type of currency currently in the kiosk");
            Console.WriteLine();
            for (int index = 0; index < kiosk.CurrencyAmount.Length; index++)
            {
                Console.WriteLine(kiosk.CurrencyName[index] + " " + kiosk.CurrencyAmount[index]);
            }
            Console.ForegroundColor = ConsoleColor.White;

            //Reset kiosk for the next customer
            for (int i = 0; i < kiosk.UserPayment.Length; i++)
            {
                //Reset values stored for bills/coins the user entered to zero
                kiosk.UserPayment[i] = 0;
            }
            Console.WriteLine("______________________________");
            Console.WriteLine();


            DateTime dtmTime;
            dtmTime = DateTime.Now;

            trans.transactionDate = dtmTime.ToString("MMM-dd-yyyy,HH:mm");
            trans.cashAmount = 0;
            trans.cardVendor = "No Vendor Given";
            trans.cardAmount = 0;
            trans.changeGiven = 0;

        }//End of Transaction Reset
        #endregion

        #region Logging
        static void Logging()
        {
            //Replacing spaces with | for formatting
            string vendor = trans.cardVendor.Replace(' ', '_');

            //Putting variables into a single string
            string output = trans.transactionNum.ToString() + "," + trans.transactionDate + ",$" + trans.cashAmount.ToString() 
                            + "," + vendor + ",$" + trans.cardAmount + ",$" + trans.changeGiven.ToString(); 

            //Running the logging package
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "Logging Project.exe";
            startInfo.Arguments = output;
            Process.Start(startInfo);
        }                         

        #endregion                
                                  
    } //End of Functions

} //End of Program