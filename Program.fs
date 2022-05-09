open System
open AssemblyLoadContextDemo.Core.loading
[<EntryPoint>]
let main args = 
    
    let result = loadAndUnload()
    [1 .. 10]
    |> List.iter (fun _ -> 
        if result.Value.IsAlive then
            GC.Collect()
            GC.WaitForPendingFinalizers()
        else
            ())
    printf "isalive: %b\n" result.Value.IsAlive

    let result2 = loadAndUnloadOneMethod()
    [1 .. 10]
    |> List.iter (fun _ -> 
        if result2.IsAlive then
            GC.Collect()
            GC.WaitForPendingFinalizers()
        else
            ())

    printf "isalive2: %b\n" result2.IsAlive
    0