using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf
{
    internal static class ChatStore
    {
        private const string FileName = "chats.xml";

        private static readonly object Lock = "Lock";

        public static void StoreChats(IEnumerable<IPublicChatViewModel> chatRooms)
        {
            lock (Lock)
            {
                try
                {
                    using (var fileStream = new FileStream(FileName, FileMode.Create))
                    {
                        var root = new XElement("Chats");
                        foreach (var chatRoom in chatRooms)
                        {
                            var room = new XElement("ChatRoom");
                            room.SetAttributeValue("Name", chatRoom.Name);
                            room.SetAttributeValue("PSK", Convert.ToBase64String(chatRoom.PreSharedKey));
                            room.SetAttributeValue("LastActivity", chatRoom.LastActivity);
                            root.Add(room);
                        }
                        var document = new XDocument(root);
                        document.Save(fileStream);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public static IEnumerable<ChatStoreModel> LoadChats()
        {
            lock (Lock)
            {
                FileStream fileStream;
                XDocument document;
                try
                {
                    if (!File.Exists(FileName))
                        yield break;
                    fileStream = new FileStream(FileName, FileMode.Open);
                    document = XDocument.Load(fileStream);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                using (fileStream)
                {
                    if (document.Root?.Name != "Chats")
                        yield break;

                    foreach (var element in document.Root.Elements())
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
            }
        }

        public class ChatStoreModel
        {
            public string Name { get; set; }
            public byte[] PreSharedKey { get; set; }
            public DateTime LastActivity { get; set; }
        }
    }
}