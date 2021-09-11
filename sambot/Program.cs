using System;
using AIMLbot;
using System.Speech.Synthesis;


namespace sambot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Sam Bot";
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            
            Bot myBot = new Bot();
            myBot.loadSettings();
            User myUser = new User("consoleUser", myBot);
            myBot.isAcceptingUserInput = false;
            myBot.loadAIMLFromFiles();
            myBot.isAcceptingUserInput = true;
            

            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write("Human: ");
                Console.ResetColor();
                string input = Console.ReadLine();
                if (input.ToLower() == "quit")
                {
                    Console.WriteLine("Good bye human :(");
                    break;
                    
                }
                if (input.ToLower() == "1776")
                {
                    string clearscreen = "echo hello";
                    System.Diagnostics.Process.Start("cmd.exe", clearscreen);
                }

                if (input.ToLower() == "end")
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("3");
                    Console.WriteLine("2");
                    Console.WriteLine("1");
                    synth.Speak("Stopping");
                    Console.ResetColor();
                    break;

                }




                if (input.ToLower() == "sleep now")
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.Write("Putting Display To Sleep In Three Seconds");
                    synth.Speak("Putting Display To Sleep In Three Seconds");

                    System.Threading.Thread.Sleep(3000);

                    System.Diagnostics.Process.Start("sleeptime.bat");                    
                    Console.ResetColor();
                }
                if (input.ToLower() == "cls")
                {
                    Console.Clear();
                }
                if (input.ToLower() == "clear")
                {
                    Console.Clear();
                }

                if (input.ToLower() == "reset 1")
                {

                    System.Diagnostics.Process.Start("network_reset.bat");

                }







                if (input.ToLower() == "lock")
                {
                    string strCmdText;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.Write("Locking System");
                    synth.Speak("Locking System");
                    strCmdText = "User32.dll,LockWorkStation";
                    System.Diagnostics.Process.Start("Rundll32.exe", strCmdText);
                    Console.ResetColor();
                }

                if (input.ToLower() == "ip1")
                {
                    string strCmdText;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.Write("Locking System");
                    synth.Speak("Locking System");
                    
               
                    Console.ResetColor();
                }




















                else
                {
                    Request r = new Request(input, myUser, myBot);
                    Result res = myBot.Chat(r);
                    Console.BackgroundColor = ConsoleColor.Blue;
                  
                    Console.WriteLine("Sam: " + res.Output);
                    synth.Speak("" + res.Output);
                    Console.ResetColor();

                }
            }


        }
    }
}
