namespace StickerThiefTelegramBot.Persistence

open MongoDB.Bson
open MongoDB.Driver

module Database =
    
    type BotMemory = {
        Id: ObjectId option
        UserId : int64
        StickerPackId: string
    }
    
    [<AllowNullLiteral>]
    type BotMemoryNullable = class
        public new () =
             BotMemoryNullable()
        member this.Id
            with public get(): ObjectId = this.Id
            and public set(value: ObjectId) = this.Id <- value
        member this.UserId
            with public get(): int64 = this.UserId
            and public set(value: int64) = this.UserId <- value
        member this.StickerPackId
            with public get(): string = this.StickerPackId
            and public set(value: string) = this.StickerPackId <- value
    end 
    
    let [<Literal>] ConnectionString = "mongodb+srv://root:caraCactus47@cluster0.dq5cp.mongodb.net/StickerCollectorTelegramBot?retryWrites=true&w=majority"

    let [<Literal>] DbName = "StickerCollectorTelegramBot"
    
    let [<Literal>] CollectionName = "BotMemory"

    let client              = MongoClient(ConnectionString)
    let db                  = client.GetDatabase(DbName)
    let botMemoryCollection = db.GetCollection<BotMemoryNullable>(CollectionName)

    let createInMongo (user : BotMemory ) =
        let dbUser = BotMemoryNullable()
        dbUser.UserId <- user.UserId
        dbUser.StickerPackId <- user.StickerPackId
        botMemoryCollection.InsertOne(dbUser)
        
    let getByUserIdInMogo (id:int64) =
        let mutable result: BotMemoryNullable = null
        try
            result <- (botMemoryCollection.Find(fun u -> u.UserId = id)).FirstOrDefault()
        with
            | :? System.Exception as ex ->
                printfn "TRUE: %s" (ex.ToString())
                result <- null
            
        match result with
        | null -> None
        | user -> Some { Id = Some user.Id; UserId = user.UserId; StickerPackId = user.StickerPackId }
        
        