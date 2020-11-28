open System
open System.IO
open Utilities
open Moves
open Setup
open Board

[<EntryPoint>]
let main argv =
    // Read the command line arguments to see what configuration file was loaded, and optionally, a move file

    // Test board creation
    let mutable path = new DirectoryInfo (__SOURCE_DIRECTORY__)
    while path.Name <> "play-by-email" && path.Name <> "PBEM" do
        path <- path.Parent

    let mutable board = CreateBoardFromFile (PrependSourcePath "SampleMap.txt")

    // Test unit creation
    board <- ProcessSetupFromFile (PrependSourcePath "SampleSetup.txt") board
    VisualizeMap board

    // Test unit moves
    board <- ProcessMovesFromFile (PrependSourcePath "SampleMoves.txt") board
    VisualizeMap board

    Console.WriteLine "Press any key to continue"
    Console.ReadKey () |> ignore
    0 // return an integer exit code
