namespace AssemblyLoadContextDemo.Gui
open AssemblyLoadContextDemo.Core.loading
open AssemblyLoadContextDemo.Core
open System
module main =
    let path = @"C:\Users\matthijs.prent\OneDrive - Salland Engineering\Documents\Salland\Products\Test Runner\src\TestRunner\ExampleProjects\Csharp\VeryBasics\bin\x64\Debug\net6.0\VeryBasics.dll"
    type Model =
        {
            result: string
            HostassLoadCont: HostAssemblyLoadContext option
            Weakref: WeakReference option
        }

    let init () =
        {
            result = "Nothing"
            HostassLoadCont = None
            Weakref = None
        }

    type Msg = 
    | LoadAssembly
    | UnloadAssembly
    | CollectGarbage

    let update msg m =
        match msg with
        | LoadAssembly -> 
            let ass, alc, weakref = loadAssembly path
            { m with result = "Assembly loaded!"
                     HostassLoadCont = alc
                     Weakref = weakref
            }
        | UnloadAssembly -> 
            let weakf = unloadAssembly m.HostassLoadCont m.Weakref
            
            { m with result = "isalive: " + m.Weakref.Value.IsAlive.ToString()
                     HostassLoadCont = None}
        | CollectGarbage -> 
            collectGarbage m.Weakref
            { m with result = "alive: " + m.Weakref.Value.IsAlive.ToString()}

    open Elmish.WPF

    let bindings () =
        [
            "Load" |> Binding.cmd LoadAssembly
            "Unload" |> Binding.cmd UnloadAssembly
            "result" |> Binding.oneWay (fun m -> m.result)
            "CollectGarbage" |>  Binding.cmd CollectGarbage
        ]

    let main window =
        WpfProgram.mkSimple init update bindings
        |> WpfProgram.startElmishLoop window