using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Telegram.Bot.Types.InputFiles;

namespace StickerCollectorTelegramBot
{
    class User
    {
        public ObjectId Id { get; set; }
        public long UserId { get; set; }
        public string StickerPackId { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var db = new MongoClient("mongodb+srv://root:caraCactus47@cluster0.dq5cp.mongodb.net/StickerCollectorTelegramBot?retryWrites=true&w=majority")
                .GetDatabase("StickerCollectorTelegramBot");
            var dbUsers = db.GetCollection<User>("BotMemory");
            
            var bot = new Telegram.Bot.TelegramBotClient("1697116522:AAHalShSQhFWHlv72nlgbyJeETBH76oyzJc");

            bot.OnUpdate += async (sender, eventArgs) =>
            {
                var botName = (await bot.GetMeAsync()).Username;
                var userId = eventArgs.Update.Message.From.Id;
                var sticker = eventArgs.Update.Message.Sticker;
                var packName = $"users_{userId}_pack_by_{botName}";
                
                try
                {
                    if ((await dbUsers.FindAsync(u => u.StickerPackId == packName)).Any())
                    {
                        await bot.AddStickerToSetAsync(userId, packName, sticker.FileId, sticker.Emoji);
                        await bot.SendTextMessageAsync(userId, "Added sticker to your pack");
                    }
                    else
                    {
                        var a = new InputOnlineFile(sticker.FileId);
                        
                        await bot.CreateNewStickerSetAsync(userId, packName, "TestStickerPack", a,
                            sticker.Emoji);
                        await dbUsers.InsertOneAsync(new User {UserId = userId, StickerPackId = packName});
                        await bot.SendTextMessageAsync(userId, "Created a new pack for you and added this sticker");
                    }
                }
                catch (Exception e)
                {
                    var message = e.Message.Length > 4096 ? e.Message.Substring(0, 4095) : e.Message;
                    await bot.SendTextMessageAsync(userId, message); // max message len
                }
                
                await bot.SendTextMessageAsync(userId, $"https://t.me/addstickers/{packName}");
            };
            bot.StartReceiving();
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}