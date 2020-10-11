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
                    Username = "Boxingintf2";     //Username
                    Password = "Ritigoxowok1.";   //Password

            // Authentication Type:
            // Either Manually fill in the Authentication code each time you start the application (AuthType.ManualAuthcode)
            // or Automatically enter the Authentication code (AuthType.AutomaticAuthcode) W.I.P.
            AuthenticationType = AuthType.ManualAuthcode;
            
            //
            // END Login
            //

            //Setup Listeners
            setup.SetupListeners();

            Console.ReadLine();
        }




        //Program setup
        public static string Username;
        public static string Password;
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
