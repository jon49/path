// http://builds.libav.org/windows/release-gpl/
#load "./utils/Shell.fs"
open System
open System.IO
open System.Diagnostics
open Shell

let dir = __SOURCE_DIRECTORY__
let userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
let media = Path.Combine(userDir, "Downloads", "media")

let ``youtube-dl`` = Path.Combine(dir, "../youtube-dl", "youtube-dl.exe")

type YoutubeChoice =
    | LQ_MP3
    | HQ_MP3
    | HQ_Facebook

let youtube choice links =
    let title = """-o "%(title)s-%(id)s.%(ext)s" """
    let audio = sprintf "-f 17 %s -x --audio-format mp3 --audio-quality %i %s" title
    match choice with
    | LQ_MP3 -> audio 9 links
    | HQ_MP3 -> audio 1 links
    | HQ_Facebook -> sprintf """ -f dash_sd_src_no_ratelimit %s %s""" title links

let choiceEnumerated =
    Map(
        [| ("1", LQ_MP3)
           ("2", HQ_MP3)
           ("3", HQ_Facebook) |] )

let printChoices =
    choiceEnumerated
    |> Map.fold (fun acc key v -> acc + sprintf "%s. %A\n" key v ) ""

let getChoice (num : string) =
    match choiceEnumerated.TryFind (num.Trim()) with
    | Some choice -> choice
    | _ -> failwith "Can only choose 1, 2, 3, or 4."

do
    printfn "
Choose file type:
%s"  printChoices
    let choice = Console.ReadLine () |> getChoice
    printfn "Enter links you would like to download:"
    let links = Console.ReadLine () |> fun x -> x.Trim()
    Shell.execute media ``youtube-dl`` (youtube choice links) |> ignore
