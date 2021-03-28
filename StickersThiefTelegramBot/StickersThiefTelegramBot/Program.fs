open System.Collections.Concurrent
open Funogram
open Funogram.Api
open Funogram.Tools
open Funogram.Types
open Funogram.Telegram.Api
open Funogram.Telegram.Types
open Funogram.Telegram.Bot
open StickersThiefTelegramBot.Persistence
open StickerThiefTelegramBot.Persistence.Database

let [<Literal>] telegramBotToken = "1697116522:AAHalShSQhFWHlv72nlgbyJeETBH76oyzJc" // todo: export to env variables

let fromId context =
    context.Update.Message |> Option.bind (fun m ->
        m.From |> Option.map (fun f ->
            f.Id))

let fromChatId id = ChatId.Int(id)

// Option<Sticker> == sticker option 
let stickerFromMessage (context: UpdateContext): Sticker option =
    context.Update.Message |> Option.bind (fun m ->
        m.Sticker)
   
let processResultWithValue (result: Result<'a, ApiResponseError>) =
    match result with
    | Ok v -> Some v
    | Result.Error e ->
          printfn "Server error: %s" e.Description
          None
let processResult (result: Result<'a, ApiResponseError>) =
    processResultWithValue result |> ignore


let onUpdate (context: UpdateContext) =

    let helpText = """
Hello,
Just sent me a sticker,
I will add it to your sticker pack,
for features requests or to give feedback call Valera    
good luck!
"""


    
    let botUserName = match context.Me.Username with
                      | Some username -> username
                      | None -> ""
    
    let botResult data =
        let result = api context.Config data |> Async.RunSynchronously
        result
        
    let bot data = botResult data |> processResult
    let fromId =
        match fromId context with
        | Some id -> id
        | None -> 0L // can we not have chat id?

    let sendText text =
        bot (sendMessage fromId text)

    match stickerFromMessage context with
    | Some s->
        match getByUserId fromId with
        | Some user ->
            sendText "adding sticker to your pack"
            bot (addStickerToSet fromId user.StickerPackId { FileId = s.FileId; FileSize = None; FilePath = None } s.Emoji.Value)
        | None ->
            botResult (createNewStickerSetBase fromId packName "ExtendedFavourites"
                           { FileId = s.FileId; FileSize = None; FilePath = None } s.Emoji.Value (Some false) (None))
                                |> Result.bind (fun success ->
                if success then create { UserId = fromId; StickerPackId = "title"; Id = None }
                sendText "created new sicker pack with your sticker"
                sendText (sprintf "https://t.me/addstickers/%s" packName)
                Result.Ok true) |> ignore
    | None -> sendText helpText

    
    let packName = sprintf "pack_by_%s" botUserName 
    sendText "test"
   

let startBot = startBot { defaultConfig with Token = telegramBotToken; } onUpdate None

[<EntryPoint>]
let main argv =
    
    startBot |> Async.RunSynchronously
    
    0 // return an integer exit code