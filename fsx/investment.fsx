
open System
open System.Text.RegularExpressions

let rec readlines () = seq {
    let line = Console.ReadLine()
    if line <> null then
        yield line
        yield! readlines ()
}

type Bucket =
    | MidCapBlend
    | MicroCap
    | InternationalSmall
    | PreciousMetal
    | LongTermBond
    | Cash
    | DigitalCurrency
    | Unknown
    member this.TargetPercentage () =
        match this with
        | MidCapBlend -> 0.15m
        | MicroCap -> 0.15m
        | InternationalSmall -> 0.15m
        | PreciousMetal -> 0.15m
        | LongTermBond -> 0.2m
        | Cash         -> 0.15m
        | DigitalCurrency -> 0.05m
        | Unknown -> 0.0m

let bucketList =
    [ MidCapBlend
      MicroCap
      InternationalSmall
      PreciousMetal
      LongTermBond
      Cash
      DigitalCurrency
      Unknown ]

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

//let CreateBucketList map (commodityName, balance) =
//    let bucket =
//        match buckets.TryFind commodityName with
//        | Some x -> x
//        | None -> Unknown
//    let 

let bucketName (name, _) =
    match buckets.TryFind name with
    | Some x -> x
    | None -> Unknown

let balance =
    readlines ()
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

balance
|> Map.iter (fun name amount -> printfn "%-18s: %10M - %5.2f%% - to reach target %10M" (name.ToString()) amount (amount/total*100m) 1m)

printfn "Total: %M" total

//|> Seq.iter (printfn "'%A'")
