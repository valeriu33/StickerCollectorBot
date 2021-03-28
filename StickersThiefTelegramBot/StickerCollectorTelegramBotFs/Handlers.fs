module StickerCollectorTelegramBotFs.Handlers

open System.Text.Unicode
open Funogram
open Funogram.Telegram.Bot
open Funogram.Types
open Funogram.Telegram.Api
open Funogram.Api
open Funogram.Telegram.Types
open Funogram.Telegram.Bot
open Funogram.Telegram.Types
open Funogram.Telegram.RequestsTypes
open MongoDB.Bson
open StickerCollectorTelegramBotFs.Database

module CommandHandlers =
    
    let handleStart context =
        api context.Config (sendMessage context.Update.Message.Value.From.Value.Id "help") // todo: formId
        |> Async.RunSynchronously
        |> ignore
        ()
        
    let handleHelp context =
        ()
        
    let handleAddSticker context =
        ()
        
    let handleRemoveSticker context =
        ()
        
    let handleReplaceSticker context =
        ()

module MessageHandlers =
    let handleSticker context sticker fromId botName =
        let packName = Helpers.getStickerPackName fromId botName
        let emoji = Helpers.emptyIfNone sticker.Emoji
        
        match Mongo.getStickerPackByUserId fromId |> Async.RunSynchronously with
        | Some stickerPack ->
            (api context.Config
                (addStickerToSet
                    fromId stickerPack.StickerPackName (sticker |> Helpers.Sticker.toFile) emoji)
            ) |> Async.RunSynchronously |> ignore
            (api context.Config
                (sendMessage fromId ("Created sticker pack, add it by the link: " + "https://t.me/addstickers/" + packName))
            ) |> Async.RunSynchronously |> ignore
        | None ->
            let fileResult = (api context.Config (
                            getFile sticker.FileId) |> Async.RunSynchronously)
            let file = match fileResult with
            | Ok file -> Some file
            | Error apiError -> None
            
            let newFile =
                {
                    FileId = file.Value.FileId
                    FilePath = Some (sprintf "https://api.telegram.org/file/bot%s/%s" context.Config.Token file.Value.FilePath.Value)
                    FileSize = file.Value.FileSize
                }
            
            let result = ((api context.Config
                            (createNewStickerSetBase
                                 fromId packName "FavouritesExtended" file.Value emoji (Some false) None)
                            ) |> Async.RunSynchronously)
            match result with
            | Ok _ ->
                    Mongo.createStickerPack { Id = ObjectId.Empty; UserId = fromId; StickerPackName = packName } |> Async.RunSynchronously
                    (api context.Config (sendMessage fromId (sprintf "https://t.me/addstickers/%s" packName))) |> Async.RunSynchronously |> ignore
            | Error e ->
                (api context.Config (sendMessage fromId (sprintf "Huston, we have a problem: %s" e.Description))) |> Async.RunSynchronously |> ignore
            