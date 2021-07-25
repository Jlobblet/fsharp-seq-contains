module fsharp_seq_contains.NewCode

/// Copied from
/// https://github.com/dotnet/fsharp/blob/d6fba3ae20c4ae8c9382eb0e65d956635e1e22f2/src/fsharp/FSharp.Core/seqcore.fs#L127-L129
let inline checkNonNull argName arg = if isNull arg then nullArg argName

/// Adapted from
/// https://github.com/dotnet/fsharp/blob/d6fba3ae20c4ae8c9382eb0e65d956635e1e22f2/src/fsharp/FSharp.Core/seq.fs#L535-L542
let inline Contains value (source: seq<'T>) =
    match source with
    | :? System.Collections.Generic.ICollection<'T> as ic -> ic.Contains value
    | _ ->
        checkNonNull (nameof source) source
        use e = source.GetEnumerator()
        let mutable state = false

        while (not state && e.MoveNext()) do
            state <- value = e.Current

        state
