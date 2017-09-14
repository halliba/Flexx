using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Flexx.Wpf.Annotations;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf
{
    internal static class ChatStore
    {
        private const string FileName = "chatstore.xml";

        private static readonly object Lock = "Lock";

        public static void Store(IEnumerable<IPublicChatViewModel> chatRooms, IEnumerable<IChatPartnerViewModel> chatPartners)
        {
            lock (Lock)
            {
                try
                {
                    var chats = new XElement("Chats");
                    foreach (var chatRoom in chatRooms)
                    {
                        var room = new XElement("ChatRoom");
                        room.SetAttributeValue("Name", chatRoom.Name);
                        room.SetAttributeValue("PSK", Convert.ToBase64String(chatRoom.PreSharedKey));
                        room.SetAttributeValue("LastActivity", chatRoom.LastActivity);
                        chats.Add(room);
                    }

                    var users = new XElement("Users");
                    foreach (var chatPartner in chatPartners)
                    {
                        var user = new XElement("User");
                        user.SetAttributeValue("Name", chatPartner.Name);
                        user.SetAttributeValue("PublicKey", chatPartner.PublicKey);
                        user.SetAttributeValue("LastActivity", chatPartner.LastActivity);
                        users.Add(user);
                    }
                        
                    var root = new XElement("FLEXX");
                    root.Add(chats);
                    root.Add(users);
                    var document = new XDocument(root);

                    using (var fileStream = new FileStream(FileName, FileMode.Create))
                    {
                        document.Save(fileStream);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public static ChatStoreGroup Load()
        {
            lock (Lock)
            {
                XDocument document;
                try
                {
                    if (!File.Exists(FileName))
                        return null;
                    var fileStream = new FileStream(FileName, FileMode.Open);
                    using (fileStream)
                    {
                        document = XDocument.Load(fileStream);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                if (document.Root?.Name != "FLEXX")
                    return null;
                
                List<ChatStoreModel> chats = null;
                try
                {

                    var chatRoot = document.Root.Element("Chats");
                    if (chatRoot != null)
                        chats = LoadChats(chatRoot)?.ToList();
                }
                catch (Exception)
                {
                    // Ignored
                }

                List<UserStoreModel> users = null;
                try
                {
                    var userRoot = document.Root.Element("Users");
                    if (userRoot != null)
                        users = LoadUsers(userRoot)?.ToList();
                }
                catch (Exception)
                {
                    // Ignored
                }

                return new ChatStoreGroup
                {
                    Chats = chats,
                    Users = users
                };
            }
        }

        private static IEnumerable<ChatStoreModel> LoadChats(XContainer chatRoot)
        {
            if (chatRoot == null)
                yield break;
            
            foreach (var element in chatRoot.Elements("ChatRoom"))
            {
                var chat = new ChatStoreModel();
                var name = element.Attribute("Name")?.Value;
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                var pskString = element.Attribute("PSK")?.Value;
                if (string.IsNullOrWhiteSpace(pskString))
                    continue;
                var psk = Convert.FromBase64String(pskString);

                var lastActivityString = element.Attribute("LastActivity")?.Value;
                if (string.IsNullOrWhiteSpace(lastActivityString) ||
                    !DateTime.TryParse(lastActivityString, out var lastActivity))
                    continue;

                chat.Name = name;
                chat.PreSharedKey = psk;
                chat.LastActivity = lastActivity;
                yield return chat;
            }
        }

        private static IEnumerable<UserStoreModel> LoadUsers(XContainer userRoot)
        {
            if (userRoot == null)
                yield break;

            foreach (var element in userRoot.Elements("User"))
            {
                var name = element.Attribute("Name")?.Value;
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                var publicKey = element.Attribute("PublicKey")?.Value;
                if (string.IsNullOrWhiteSpace(publicKey))
                    continue;

                var lastActivityString = element.Attribute("LastActivity")?.Value;
                if (string.IsNullOrWhiteSpace(lastActivityString) ||
                    !DateTime.TryParse(lastActivityString, out var lastActivity))
                    continue;

                var user = new UserStoreModel
                {
                    Name = name,
                    PublicKey = publicKey,
                    LastActivity = lastActivity
                };
                yield return user;
            }
        }

        public class ChatStoreGroup
        {
            public List<ChatStoreModel> Chats { get; set; }
            public List<UserStoreModel> Users { get; set; }
        }

        public class ChatStoreModel
        {
            public string Name { get; set; }
            public byte[] PreSharedKey { get; set; }
            public DateTime LastActivity { get; set; }
        }

        public class UserStoreModel
        {
            public string Name { get; set; }
            public string PublicKey { get; set; }
            public DateTime LastActivity { get; set; }
        }
    }
}