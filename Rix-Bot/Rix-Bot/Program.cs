using System;
using System.IO;

namespace Rix_Bot
{
    public class Program
    {
        //Here you can adjust your settings
        static void Main(string[] args)
        {
            Setup setup = new Setup();

            //
            // Login
            //

            // Login type: 
            // Either manually login each time you start the application (LoginType.ManualLoginDetails) 
            // Automatically login, but manually login the first time you start the application (LoginType.OneTimeManualDetails)
            // or Automatically login with the Username and password set in the code below (LoginType.BuiltinLoginDetails)
            LoginDetailType = LoginType.BuiltinLoginDetails;

                //these login details are used if LoginDetailType = LoginType.BuiltinLoginDetails
                    Username = "boxingintf2";     //Username
                    Password = "Ritigoxowok1.";   //Password

            // Authentication Type:
            // Either Manually fill in the Authentication code each time you start the application (AuthType.ManualAuthcode)
            // or Automatically Authenticate by only filling in the Authentication code one time (AuthType.AutomaticAuthcode)
            AuthenticationType = AuthType.AutomaticAuthcode;
            //Note If you use AutomaticAuthCode it will make a file called LoginKey.txt

            //
            // Disconnection
            //

            //Reconnecting:
            //If you want your bot to reconnect if it disconnected somehow set ReconnectAfterDisconnect to true
            //Otherwise make it False if you want the bot to stay disconnected
            //ReconnectionAttempts determines the ammount of tries the bot has to reconnect
                ReconnectAfterDisconnect = true;
                ReconnectionAttempts = 3;

            //Setup Listeners
            setup.SetupListeners();

            Console.ReadLine();
        }




        //Program setup

        //Login
        public static string Username;
        public static string Password;

        //Disconnection
        public static bool ReconnectAfterDisconnect = true;
        public static int ReconnectionAttempts;
        private Setup setup;

        public Program(Setup setup)
        {
            this.setup = setup;
        }

        public static LoginType LoginDetailType;
        public enum LoginType
        {
            ManualLoginDetails,
            OneTimeManualDetails,
            BuiltinLoginDetails
        }

        public static AuthType AuthenticationType;
        public enum AuthType
        {
            ManualAuthcode,
            AutomaticAuthcode,
            Disabled
        }
    }
}
