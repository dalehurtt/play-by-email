open System
open Moves
open Setup
open Board

[<EntryPoint>]
let main argv =
    // Read the command line arguments to see what configuration file was loaded, and optionally, a move file

    // Test board creation
    let path = __SOURCE_DIRECTORY__
    let mutable board = CreateBoardFromFile (sprintf @"%s\SampleMap.txt" path)

    // Test unit creation
    board <- ProcessSetupFromFile (sprintf @"%s\SampleSetup.txt" path) board
    VisualizeMap board

    // Test unit moves
    board <- ProcessMovesFromFile (sprintf @"%s\SampleMoves.txt" path) board
    VisualizeMap board

    Console.WriteLine "Press any key to continue"
    Console.ReadKey () |> ignore
    0 // return an integer exit code
