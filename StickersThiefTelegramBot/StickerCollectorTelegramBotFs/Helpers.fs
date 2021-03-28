module StickerCollectorTelegramBotFs.Helpers

open Funogram.Api
open Funogram.Telegram.Api
open Funogram.Telegram.Bot

let bot config data = api config data

let getStickerPackName fromId botName = sprintf "users_%s_pack_by_%s" (fromId.ToString()) botName

let sendMessageIfNone context fromId opt =
    match opt with
    | None -> bot context.Config (sendMessage fromId "Some problem occured, try again") |> Async.RunSynchronously |> ignore
    | _ -> () 

let emptyIfNone = function
    | Some s -> s
    | None -> ""

module Sticker =
    open Funogram.Telegram.Types
    let toFile (sticker: Sticker) : File =
        { FileId = sticker.FileId; FileSize = None; FilePath = None }