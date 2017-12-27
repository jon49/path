#load "./utils/Utils.fs"

open System
open System.Text.RegularExpressions
open Utils
open System.IO

let debug = false

let log a =
    if debug then (printfn "%A" a)
    a

type Bucket =
    | MidCapBlend
    | MicroCap
    | InternationalSmall
    | PreciousMetal
    | LongTermBond
    | Cash
    | DigitalCurrency
    | Unknown
    static member ToList () =
        [ MidCapBlend
          MicroCap
          InternationalSmall
          PreciousMetal
          LongTermBond
          Cash
          DigitalCurrency
          Unknown ]
    member this.TargetRatio () =
        match this with
        | MidCapBlend -> 0.15m
        | MicroCap -> 0.15m
        | InternationalSmall -> 0.15m
        | PreciousMetal -> 0.15m
        | LongTermBond -> 0.20m
        | Cash -> 0.15m
        | DigitalCurrency -> 0.05m
        | Unknown -> 0m
    member this.TargetPercentage () =
        this.TargetRatio () |> (*) 100m

let buckets =
    Map.empty<string, Bucket>
    |> Map.add "VO" MidCapBlend // (* Vanguard Mid-Cap ETF *) 15%
    //|> Map.add MicroCap // 0.15m
    |> Map.add "VSS" InternationalSmall // 15% (* Vanguard FTSE All Wd Ex US Small Cap *)
    |> Map.add "Gold" PreciousMetal // 15% Gold
    |> Map.add "Silver" PreciousMetal // Silver
    //|> Map.add LongTermBond    // 20%
    |> Map.add "Cash" Cash      // 17.5%
    |> Map.add "BTC" DigitalCurrency // 2.5%
    |> Map.add "BCH" DigitalCurrency

let tickers =
    buckets
    |> Map.toArray
    |> Array.map fst
    |> set

let rawBalance =
    { (Shell.create "hledger.exe") with
        Arguments = sprintf "b %s -V -N --flat" (tickers |> String.concat " ") }
    |> Shell.read
    |> Seq.map log

let calculateDifference total (bucket : Bucket) currentValue =
    let percentTarget = bucket.TargetRatio()
    let currentPercent = currentValue / total
    (percentTarget - currentPercent) * total

let bucketName (name, _) =
    match buckets.TryFind name with
    | Some x -> x
    | None -> Unknown

let balance =
    rawBalance
    |> Seq.map (fun x ->
        let xs = Regex.Split(x, "\s+")
        let balance = decimal <| xs.[1].Replace("$", "").Replace(",", "")
        let name = xs.[2]
        let start = name.LastIndexOf(":") + 1
        name.[start..], balance )
    |> Seq.groupBy bucketName
    |> Seq.fold (fun map (bucket, xs) -> Map.add bucket (xs |> Seq.map snd |> Seq.sum) map) Map.empty<Bucket, decimal>

let total =
    balance
    |> Map.toArray
    |> Array.sumBy snd

let money (space : string option) (tw : TextWriter) (x : decimal) = 
    tw.Write("{"+(space |> Option.map(fun x -> "0,"+x) |> Option.defaultValue "0")+":#,0.00}", x)

let money10 a b = money (Some "10") a b
let money0 a b  = money None a b

//balance
printfn "\n Bucket           | Amount              |  To Reach Target Amount "
printfn   "------------------|---------------------|-------------------------"
Bucket.ToList ()
|> List.iter (fun bucket ->
    let amount = balance.TryFind bucket |> Option.defaultValue 0m
    printfn "%-18s| %a   %5.2f%% | %a   %5.2f%%" (bucket.ToString()) money10 amount (amount/total*100m) money10 (calculateDifference total bucket amount) (bucket.TargetPercentage())
)

printfn "\nTotal: $%a" money0 total

//|> Seq.iter (printfn "'%A'")
