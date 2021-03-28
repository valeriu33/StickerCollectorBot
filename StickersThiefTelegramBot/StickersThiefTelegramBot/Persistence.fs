module StickersThiefTelegramBot.Persistence

open StickerThiefTelegramBot.Persistence.Database

//int64 -> BotMemory option

type Repository =
    | AddNewStickerPack of (BotMemory -> unit)
    | GetStickerPack of (int64 -> BotMemory)