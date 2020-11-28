open System
open System.IO
open Moves
open Setup
open Board

[<EntryPoint>]
let main argv =
    // Read the command line arguments to see what configuration file was loaded, and optionally, a move file

    // Test board creation
    let mutable path = new DirectoryInfo (__SOURCE_DIRECTORY__)
    while path.Name <> "play-by-email" do
        path <- path.Parent

    //printfn "%s" (Path.Combine (path.FullName, "SampleMap.txt"))
    let mutable board = CreateBoardFromFile (Path.Combine (path.FullName, "SampleMap.txt"))

    // Test unit creation
    board <- ProcessSetupFromFile (Path.Combine (path.FullName, "SampleSetup.txt")) board
    VisualizeMap board

    // Test unit moves
    //board <- ProcessMovesFromFile (sprintf @"%s\SampleMoves.txt" path.FullName) board
    //VisualizeMap board

    Console.WriteLine "Press any key to continue"
    Console.ReadKey () |> ignore
    0 // return an integer exit code
