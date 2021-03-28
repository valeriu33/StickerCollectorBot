module StickerCollectorBotFs

open Funogram.Telegram.Bot
open MongoDB.Bson
open StickerCollectorTelegramBotFs.Database
open StickerCollectorTelegramBotFs.Handlers
open StickerCollectorTelegramBotFs.Helpers
open System.IO
open Funogram.Api
open Funogram.Types
open Funogram.Telegram.Api
open Funogram.Telegram.Types
open Funogram.Telegram.Bot
open System.Net

let onUpdate context =
    let getFromId =
        context.Update.Message |> Option.bind (fun m ->
            m.From |> Option.map (fun f ->
                f.Id))
        
    let getFromId1 =
        context.Update.Message
        |> Option.bind (fun m -> m.From)
        |> Option.map (fun u -> u.Id)
        
    if getFromId = None then ()
    
    let fromId = getFromId.Value
    
    let botName =
        match context.Me.Username with
        | Some a -> a
        | None -> ""
    
    match context.Update.Message with
    | Some message when message.Text <> None ->
        match message.Text with
         | Some command when command.[0] = '/' ->
             match command with
             | "/start" -> CommandHandlers.handleStart context
             | "/help" -> CommandHandlers.handleHelp context
             | "/addSticker" -> CommandHandlers.handleAddSticker context
             | "/removeSticker" -> CommandHandlers.handleRemoveSticker context
             | "/replaceSticker" -> CommandHandlers.handleReplaceSticker context
             | _ -> failwith "todo"
         | _ -> failwith "todo"
    | Some message when message.Sticker <> None ->
         match message.Sticker with
         | Some sticker -> MessageHandlers.handleSticker context sticker fromId botName
         | None -> ()
    | None -> ()
    | _ -> failwith "todo"

let startBot = startBot { defaultConfig with Token = "1697116522:AAHalShSQhFWHlv72nlgbyJeETBH76oyzJc"; } onUpdate None 

[<EntryPoint>]
let main argv =
    startBot |> Async.RunSynchronously
    printfn "Hello world" 
    0