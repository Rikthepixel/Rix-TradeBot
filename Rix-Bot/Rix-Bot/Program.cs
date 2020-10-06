using System.IO;

namespace Rix_Bot
{
    public class Program
    {
        //Here you can adjust your settings
        static void Main(string[] args)
        {
            Setup setup = new Setup();

            // Login type: 
            // Either manually login each time you start the application (LoginType.ManualLoginDetails) 
            // Automatically login, but manually login the first time you start the application (LoginType.OneTimeManualDetails)
            // or Automatically login with the Username and password set in the code below (LoginType.BuiltinLoginDetails)
            LoginDetailType = LoginType.BuiltinLoginDetails;

                //these login details are used if LoginDetailType = LoginType.BuiltinLoginDetails
                    Username = "Tempyelectric";     //Username
                    Password = "6VZQpNGhfQc5dLq";   //Password
            
            //Setup Listeners
            setup.SetupListeners();
        }


        //Program setup
        public static string Username;
        public static string Password;
        private Setup setup;

        public Program(Setup setup)
        {
            this.setup = setup;
        }

        public enum LoginType
        {
            ManualLoginDetails,
            OneTimeManualDetails,
            BuiltinLoginDetails
        }
        public static LoginType LoginDetailType;
    }
}
