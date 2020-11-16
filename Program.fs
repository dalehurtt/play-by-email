open System
open Board
open GamePieces

[<EntryPoint>]
let main argv =
    // Read the command line arguments to see what configuration file was loaded, and optionally, a move file

    // Test board creation
    let path = __SOURCE_DIRECTORY__
    let board = CreateBoardFromFile (sprintf @"%s\SampleMap.txt" path)

    // Test unit creation
    let redinf1 = CreatePiece "Red Infanty 1" PieceType.Infantry Period.Ancient
    let blueinf1 = CreatePiece "Blue Infantry 1" PieceType.Infantry Period.Ancient

    // Test unit placement


    System.Console.ReadKey () |> ignore
    0 // return an integer exit code
