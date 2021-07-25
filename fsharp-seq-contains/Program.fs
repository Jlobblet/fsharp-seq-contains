// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open BenchmarkDotNet.Running
open fsharp_seq_contains.Benchmark

[<EntryPoint>]
let main _ =
    BenchmarkRunner.Run<ContainsComparison>()
    |> ignore

    0 // return an integer exit code
