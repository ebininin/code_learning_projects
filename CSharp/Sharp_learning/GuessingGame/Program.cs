using System;

namespace GuessingGame {
    internal class Program {
        public static void Main(string[] args) {
            string secretWord = "giraffe";
            string guess = "";
            int guessTime = 0;
            int guessLimit = 6;
            bool outtaGuesses = false;
            
            do {
                if (guessTime < guessLimit) {
                    Console.Write("Enter guess: ");
                    guess = Console.ReadLine();
                    guessTime++;
                }
                else {
                    outtaGuesses = true;
                }
 
            } while (guess != secretWord && !outtaGuesses);

            if (outtaGuesses) {
                Console.Write("You Lose");
            }
            else {
                Console.Write("You Win!");
            }
        }
    }
}