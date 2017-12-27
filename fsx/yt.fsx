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
    | HQ_MP3_SoundCloud
    | HQ_Facebook

let youtube choice links =
    let title = """-o "%(title)s-%(id)s.%(ext)s" """
    let audio = sprintf "-f 17 %s -x --audio-format mp3 --audio-quality %i %s" title
    match choice with
    | LQ_MP3 -> audio 9 links
    | HQ_MP3 -> audio 1 links
    | HQ_MP3_SoundCloud -> sprintf """ -f http_mp3_128_url -x --audio-format mp3 --audio-quality 1 %s %s""" title links
    | HQ_Facebook -> sprintf """ -f dash_sd_src_no_ratelimit %s %s""" title links

let choiceEnumerated =
    Map(
        [| ("1", LQ_MP3)
           ("2", HQ_MP3)
           ("3", HQ_MP3_SoundCloud)
           ("4", HQ_Facebook) |] )

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
    let links = Console.ReadLine () |> fun x -> x.Trim()
    Shell.execute
        { (Shell.create ``youtube-dl``) with
            WorkingDirectory = Some (if choice = LQ_MP3 then media else keep)
            Arguments = (youtube choice links)
            UseShellExecute = true
            WaitForExit = true }
