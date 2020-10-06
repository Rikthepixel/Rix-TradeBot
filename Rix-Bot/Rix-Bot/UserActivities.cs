using SteamKit2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Rix_Bot
{
    class UserActivities
    {
        private readonly Setup setup;
        public UserActivities(Setup setup)
        {
            this.setup = setup;
        }

        public void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine("Connected succesfully");



            setup.steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = setup.UserName,
                Password = setup.PassWord,

            });
        }
        //on Disconnect function
        public void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Console.WriteLine("Disconnected succesfully");

            setup.isRunning = false;
        }

        //on Logg on function
        public void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            //if failed to loggon
            if (callback.Result != EResult.OK)
            {
                // steamguard protection
                if (callback.Result == EResult.AccountLogonDenied)
                {
                    Console.WriteLine("Account is SteamGuard protected.");
                    setup.isRunning = false;
                    return;
                }

                Console.WriteLine($"Unabble to logon: {callback.Result}, {callback.ExtendedResult}.");
                setup.isRunning = false;
            }

            if (callback.Result == EResult.OK)
            {
                Console.WriteLine("Logged on succesfully.");
            }
        }

        //on Logg off function
        public void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Console.WriteLine($"Logged off succesfully {callback.Result}");
        }

        public void OnAccountInfo(SteamUser.AccountInfoCallback callback)
        {
            Console.WriteLine("Setting personastate to Online");
            setup.steamFriends.SetPersonaState(EPersonaState.Online);
        }
    }
}
