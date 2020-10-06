using System.IO;

namespace Rix_Bot
{
    public class Program
    {
        private Setup setup;

        public Program(Setup setup)
        {
            this.setup = setup;
        }

        public enum LoginType
        {
            ManualLoginDetails,
            TextfileLoginDetails,
            BuiltinLoginDetails
        }
        public static LoginType LoginDetailType;

        //Here you can adjust your settings
        static void Main(string[] args)
        {
            
            Setup setup = new Setup();

            // Login type: 
            // Either manually login (LoginType.ManualLoginDetails) 
            // automatically login which requires the setup.UserName and setup.PassWord to function (LoginType.BuiltinLoginDetails)
            // or use a textfile to store your Login details (LoginType.TextfileLoginDetails) 
            LoginDetailType = LoginType.TextfileLoginDetails;

                    //these login details are used if LoginDetailType = LoginType.ManualLoginDetails
                    setup.UserName = "Tempyelectric"; //Username
                    setup.PassWord = "6VZQpNGhfQc5dLq"; //Password
            
            //Setup Listeners
            setup.SetupListeners();
        }
    }
}
