using SteamKit2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rix_Bot
{
    class Friends
    {
        private Setup setup;
        public Friends(Setup setup)
        {
            this.setup = setup;
        }

        public void OnFriendsList(SteamFriends.FriendsListCallback callback)
        {
            int FriendCount = setup.steamFriends.GetFriendCount();
            Console.WriteLine($"I have {FriendCount} friends.");

            foreach (var friend in callback.FriendList)
            {
                SteamID ID = friend.SteamID;
                String Naam;
                Naam = setup.steamFriends.GetFriendPersonaName(ID);
                
                Console.WriteLine($"Friend: {ID}, {Naam}");
                if (friend.Relationship == EFriendRelationship.RequestRecipient)
                {
                    setup.steamFriends.AddFriend(friend.SteamID);
                }
            }
        }
        string SteamID;
        public void OnFriendAdded(SteamFriends.FriendAddedCallback callback)
        {
            SteamID = Convert.ToString(callback.SteamID);
            Console.WriteLine($"{callback.PersonaName}, {callback.SteamID} is now our friend.");
            setup.steamFriends.SendChatMessage(callback.SteamID, EChatEntryType.ChatMsg, $"Hello, I am a bot, {callback.PersonaName}. As of yet i have no functionality! :'(");
        }

        public void OnPersonaState(SteamFriends.PersonaStateCallback callback)
        {
            Console.WriteLine($"Friend state change: {callback.Name}, {callback.State}");
        }

        public void OnProfileInfo(SteamFriends.ProfileInfoCallback callback)
        {
            Console.WriteLine(callback);

        }
    }
}
