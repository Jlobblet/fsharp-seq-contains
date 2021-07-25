module fsharp_seq_contains.Benchmark

open System.Linq
open BenchmarkDotNet.Attributes
open fsharp_seq_contains.Config
open fsharp_seq_contains.NewCode

[<Struct>]
type SeqMaker =
    { Label: string
      Func: int seq -> int seq }
    static member Default() = { Label = "id"; Func = id }
    static member Create(l, f) = { Label = l; Func = f }
    override this.ToString() = this.Label

type ContainsBehaviour =
    | Start = 0
    | Middle = 1
    | End = 2
    | NotPresent = 3

[<Config(typeof<MyConfig>); CsvMeasurementsExporter; RPlotExporter>]
type ContainsComparison() =
    member val private source: int seq = null with get, set
    member val private target: int = 0 with get, set

    [<ParamsSource("Inits")>]
    member val public Init: SeqMaker = SeqMaker.Default() with get, set

    member val public Inits =
        let sqc = SeqMaker.Create

        seq {
            yield sqc ("F# Array", (fun items -> Array.ofSeq items :> int seq))
            yield sqc ("F# List", (fun items -> List.ofSeq items :> int seq))
            yield sqc ("F# Set", (fun items -> Set.ofSeq items :> int seq))
            yield sqc ("List<T>", (fun items -> System.Collections.Generic.List<int> items :> int seq))
            yield sqc ("HashSet<T>", (fun (items: int seq) -> System.Collections.Generic.HashSet<int> items :> int seq))
//            yield sqc ("LinkedList<T>", (fun items -> System.Collections.Generic.LinkedList<int> items :> int seq))
//            yield sqc ("Queue<T>", (fun items -> System.Collections.Generic.Queue<int> items :> int seq))
//            yield sqc ("SortedSet<T>", (fun items -> System.Collections.Generic.SortedSet<int> items :> int seq))
//            yield sqc ("Stack<T>", (fun items -> System.Collections.Generic.Stack<int> items :> int seq))
        }

    [<ParamsSource("NSource")>]
    member val public N = 0 with get, set

    [<ParamsAllValues>]
    member val public containsBehaviour = ContainsBehaviour.NotPresent with get, set

    member val public NSource =
        seq {
            yield 0
            yield 1
            yield 10
//            yield 100
            yield 1_000
//            yield 10_000
            yield 100_000
        }

    [<GlobalSetup>]
    member this.GlobalSetup() =
        this.source <- seq { 0 .. this.N } |> this.Init.Func

        this.target <-
            match this.containsBehaviour with
            | ContainsBehaviour.Start -> 1
            | ContainsBehaviour.Middle -> this.N / 2
            | ContainsBehaviour.End -> this.N
            | ContainsBehaviour.NotPresent -> 0
            | _ -> failwith $"Unknown value for {nameof this.containsBehaviour}: {this.containsBehaviour}"

    [<GlobalCleanup>]
    member this.GlobalCleanup() =
        this.source <- null
        this.target <- 0

    [<Benchmark>]
    member this.Current() = Seq.contains this.target this.source

    [<Benchmark>]
    member this.Proposed() = Contains this.target this.source

    [<Benchmark>]
    member this.Linq() = this.source.Contains this.target
