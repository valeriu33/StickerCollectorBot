module StickerCollectorTelegramBotFs.Database

open MongoDB.Bson
open MongoDB.Driver

type State =
    | Deleting
    | ChangingPlace
    | Adding

type StickerPack = {
    Id: ObjectId
    UserId : int64
    StickerPackName: string
}

type DialogState = {
    Id: ObjectId
    UserId: int64
    State: string
}

let [<Literal>] ConnectionString = "mongodb+srv://root:caraCactus47@cluster0.dq5cp.mongodb.net/StickerCollectorTelegramBot?retryWrites=true&w=majority"
let [<Literal>] DbName = "StickerCollectorTelegramBot"

let db = MongoClient(ConnectionString).GetDatabase(DbName)
let stickerPacks = db.GetCollection<StickerPack>("StickerPacks")
let dialogStates = db.GetCollection<DialogState>("DialogStates")

module Mongo =
    let createStickerPack (user : StickerPack ) =
        stickerPacks.InsertOneAsync(user) |> Async.AwaitTask
    
    let getStickerPackByUserId (id:int64) =
        async {
            let! result = stickerPacks.Find<StickerPack>(fun u -> u.UserId = id).ToListAsync()
                          |> Async.AwaitTask
            let resultList = result |> Seq.toList<StickerPack> // not necessary

            match resultList with
            | head::_ -> return Some head
            | [] -> return None
        }