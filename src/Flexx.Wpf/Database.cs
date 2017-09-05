//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Windows.Media;
//using Flexx.Core;
//using Flexx.Wpf.ViewModels;
//using SQLite.CodeFirst;
//using System.Collections.ObjectModel;
//using System.IO;
//using System.Reflection;
//using System.Xml.Linq;
//using Flexx.Core.Api;

//namespace Flexx.Wpf
//{
//    internal class Database
//    {
//        //private readonly ChatDbContext _context;

//        public static Database Current = new Database();
        
//        private string _userFilePath = Path.Combine(Directory.GetCurrentDirectory(), "users.xml");

//        private IEnumerable<UserIdentity> GetIdentities()
//        {
//            if (!File.Exists(_userFilePath))
//                WriteResourceToFile("Recources\\UserFileTemplate.xml", _userFilePath);
//            var xdoc = XDocument.Load(_userFilePath);
//            if (xdoc.Root?.Name != "users")
//            {
//                WriteResourceToFile("Recources\\UserFileTemplate.xml", _userFilePath);
//                xdoc = XDocument.Load(_userFilePath);
//            }

//            if (xdoc.Root == null)
//                throw InvalidOperationException("")
//            {
//                var users = xdoc.Root.Elements().Select()
//            }
//        }

//        private static void WriteResourceToFile(string resourceName, string fileName)
//        {
//            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
//            {
//                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
//                {
//                    resource?.CopyTo(file);
//                }
//            }
//        }

//        //private Database()
//        //{
//        //    _context = new ChatDbContext();
//        //}

//        //private class ChatDbContext : DbContext
//        //{
//        //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        //    {
//        //        var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<ChatDbContext>(modelBuilder);
//        //        System.Data.Entity.Database.SetInitializer(sqliteConnectionInitializer);
//        //    }

//        //    public DbSet<UserModel> Users { get; set; }
//        //}

//        //public class UserModel
//        //{
//        //    public string UserName { get; set; }

//        //    public string PublicKey { get; set; }

//        //    public Color Color { get; set; }

//        //    public ICollection<MessageModel> Messages { get; set; }
//        //}

//        //public class MessageModel
//        //{
//        //    public string Content { get; set; }

//        //    public DateTime Timestamp { get; set; }
//        //}

//        //public IEnumerable<MessageModel> GetMessages(UserIdentity identity)
//        //{
//        //    var user = _context.Users
//        //        .Include(u => u.Messages)
//        //        .FirstOrDefault(u => u.PublicKey == identity.PublicKey);
//        //    var messages = user?.Messages ?? new List<MessageModel>();
//        //    if (user != null) return messages;

//        //    user = new UserModel
//        //    {
//        //        UserName = identity.Username,
//        //        PublicKey = identity.PublicKey,
//        //        Color = UserColors.GetRandom(identity.GetHashCode())
//        //    };
//        //    _context.Users.Add(user);
//        //    _context.SaveChanges();
//        //    return messages;
//        //}

//        //public void StoreMessage(Message message)
//        //{
//        //    var user = _context.Users
//        //        .FirstOrDefault(u => u.PublicKey == message.Sender.PublicKey);
//        //    var model = new MessageModel
//        //    {

//        //    };
//        //}
//    }
//}