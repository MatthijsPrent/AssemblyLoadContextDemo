namespace AssemblyLoadContextDemo.Core
open System.Runtime.Loader
open System.Reflection
open System
open System.Runtime.CompilerServices

type HostAssemblyLoadContext (path: string) =
    inherit AssemblyLoadContext("HostAssLoadContext", true)
    let resolver: AssemblyDependencyResolver = AssemblyDependencyResolver(path)

    override u.Load (name: AssemblyName) = 
        let assemblypath = resolver.ResolveAssemblyToPath(name)
        if not( isNull assemblypath) then
            u.LoadFromAssemblyPath(assemblypath)
        else
            null


module loading =
    let loadAssembly (path: string) =
        try
            let alc = HostAssemblyLoadContext(path)
            let ass = alc.LoadFromAssemblyPath(path)
            ass |> Some ,alc |> Some, new WeakReference(alc) |> Some
        with
        | _ -> None, None, None
    
    let collectGarbage (weakref: WeakReference option) =
        match weakref with
        | None -> ()
        | Some reference ->
            [1 .. 10]
            |> List.iter (fun _ -> 
                if reference.IsAlive then
                    GC.Collect()
                    GC.WaitForPendingFinalizers()
                else
                    ()
                )

    let unloadAssembly (alc: HostAssemblyLoadContext option)(weakref: WeakReference option) =
        match alc with
        | None -> None
        | Some loadcont -> 
            loadcont.Unload()
            weakref

    let loadAndUnload () =
        let ass, alc, weakref = 
            loadAssembly @"C:\Users\matthijs.prent\OneDrive - Salland Engineering\Documents\Salland\Products\Test Runner\src\TestRunner\ExampleProjects\Csharp\VeryBasics\bin\x64\Debug\net6.0\VeryBasics.dll"
        let asslength =                 // This is to do something with the assembly to prevent it being optimized away.
            match ass with
            | Some assy -> assy.GetTypes().Length
            | None -> 0
        let result = unloadAssembly alc weakref

        collectGarbage result
        result

    



    
    let loadAndUnloadOneMethod () =
        let path = @"C:\Users\matthijs.prent\OneDrive - Salland Engineering\Documents\Salland\Products\Test Runner\src\TestRunner\ExampleProjects\Csharp\VeryBasics\bin\x64\Debug\net6.0\VeryBasics.dll"
        let alc = new HostAssemblyLoadContext(path)

        let weakref = new WeakReference(alc)

        let assembly: Assembly = alc.LoadFromAssemblyPath (path)
        
        alc.Unload()
        weakref

        
