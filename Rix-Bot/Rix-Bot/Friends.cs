using SteamKit2;
using SteamKit2.GC.TF2.Internal;
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
        public void OnFriendAdded(SteamFriends.FriendAddedCallback callback)
        {
            Console.WriteLine($"{callback.PersonaName} has become my friend!");

            //Create a response list, the randomness generator can pick one of these responses
            string[] Meet = { "Hello", "Greetings", "Good day" };

            //get a random index number for message randomness, keep messages fresh!
            Random random = new Random();
            int index = random.Next(0, Meet.Length);

            //Send a message
            setup.steamFriends.SendChatMessage(callback.SteamID, EChatEntryType.ChatMsg, $"{Meet[index]} {callback.PersonaName}, My name is {setup.steamFriends.GetPersonaName()}, I am a prototype bot. Right now, I can only be used to chat with.");
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
