open System
open Board
open GamePieces

[<EntryPoint>]
let main argv =
    // Read the command line arguments to see what configuration file was loaded, and optionally, a move file

    // Test board creation
    let path = __SOURCE_DIRECTORY__
    let board = CreateBoardFromFile (sprintf @"%s\SampleMap.txt" path)
    (*
    let cells = [
        CreateCell 1 1 Terrain.Flat (0, 0);
        CreateCell 1 2 Terrain.Flat (0, 0);
        CreateCell 2 1 Terrain.Woods (0, 0);
        CreateCell 2 2 Terrain.Flat (0, 0)
    ]
    let board = { RowCount = 2; ColCount = 2; Cells = cells}
    *)

    // Test unit creation
    let redinf1 = CreatePiece "Red Infanty 1" PieceType.Infantry Period.Ancient
    let blueinf1 = CreatePiece "Blue Infantry 1" PieceType.Infantry Period.Ancient

    // Test unit placement


    System.Console.ReadKey () |> ignore
    0 // return an integer exit code
