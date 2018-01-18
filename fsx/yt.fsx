// https://rg3.github.io/youtube-dl/download.html (Windows Executable)
// http://ffmpeg.zeranoe.com/builds/ <- for mp3 conversion. Less files than libav (below)
// http://builds.libav.org/windows/release-gpl/
#load "./utils/Utils.fs"
open System
open System.IO
open System.Diagnostics
open Utils

let dir = __SOURCE_DIRECTORY__
let userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
let media = Path.Combine(userDir, "Downloads", "media")
let keep = Path.Combine(media, "keep")

let ``youtube-dl`` = Path.Combine(dir, "../youtube-dl", "youtube-dl.exe")

type YoutubeChoice =
    | LQ_MP3
    | HQ_MP3
    | HQ_YT
    | HQ_MP3_SoundCloud
    | HQ_Facebook

let choiceArray =
    [| LQ_MP3
       HQ_MP3
       HQ_YT
       HQ_MP3_SoundCloud
       HQ_Facebook |]

let youtube choice links =
    let title = """-o "%(title)s-%(id)s.%(ext)s" """
    let audio = sprintf "-f 17 %s -x --audio-format mp3 --audio-quality %i %s" title
    match choice with
    | LQ_MP3 -> audio 9
    | HQ_MP3 -> audio 1
    | HQ_YT -> sprintf "-f 22 %s %s" title
    | HQ_MP3_SoundCloud -> sprintf """ -f http_mp3_128_url -x --audio-format mp3 --audio-quality 1 %s %s""" title
    | HQ_Facebook -> sprintf """ -f dash_sd_src_no_ratelimit %s %s""" title
    <| links

let choiceEnumerated =
    choiceArray
    |> Array.mapi (fun idx x -> string (idx + 1), x)
    |> Map

let printChoices =
    choiceEnumerated
    |> Map.fold (fun acc key v -> acc + sprintf "%s. %A\n" key v ) ""

let getChoice (num : string) =
    match choiceEnumerated.TryFind (num.Trim()) with
    | Some choice -> choice
    | _ -> failwith (sprintf "Can only choose %s." (choiceEnumerated |> Map.toArray |> Array.map (fun (x, _) -> x) |> String.concat ", "))

do

    // if directory doesn't exit create it.
    IO.Directory.CreateDirectory(keep) |> ignore

    printfn "
Choose file type:
%s"  printChoices
    let choice = Console.ReadLine () |> getChoice
    printfn "Enter links you would like to download:"
    Console.ReadLine ()
    |> fun x -> x.Trim().Split([|' '|], StringSplitOptions.RemoveEmptyEntries)
    |> Array.iter (fun link ->
        Shell.execute
            { (Shell.create ``youtube-dl``) with
                WorkingDirectory = Some (if choice = LQ_MP3 then media else keep)
                Arguments = (youtube choice link)
                UseShellExecute = true
                WaitForExit = true }
    )
