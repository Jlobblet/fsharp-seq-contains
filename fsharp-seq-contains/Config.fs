module fsharp_seq_contains.Config

open BenchmarkDotNet.Configs
open BenchmarkDotNet.Exporters
open BenchmarkDotNet.Exporters.Csv

type MyConfig() as this =
    inherit ManualConfig()

    do
        this
            .AddExporter(CsvMeasurementsExporter.Default)
            .AddExporter(RPlotExporter.Default)
        |> ignore<ManualConfig>

        ()
