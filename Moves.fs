module Moves

open System
open System.IO
open Board
open PBEM
open System.IO

let ProcessMovesFromDefinition (board : Board) (definition : string[]) : Board =
    let mutable currentSide = Piece.Side.Red
    let mutable newBoard = board
    Array.iter (fun (line : string) ->
        let parts = line.Split ([|'`'|])
        match parts.[0] with
        | "S" ->
            match parts.[1] with
            | "R" -> currentSide <- Piece.Side.Red
            | "B" -> currentSide <- Piece.Side.Blue
            | _ -> raise (new InvalidDataException (sprintf "ERROR: Parsing failure on line %s" line))
            printfn "\nProcessing moves for %O Turn %s" currentSide parts.[2]

        | "M" ->
            let (fromRow, fromCol) = (int parts.[2]), (int parts.[3])
            let (toRow, toCol) = (int parts.[4]), (int parts.[5])
            let cell = GetCell newBoard.Cells fromRow fromCol
            match cell with
            | Some c ->
                let piece = c.Piece
                match piece with
                | Some p -> newBoard <- MoveGamePiece p (fromRow, fromCol) (toRow, toCol) newBoard
                | None -> 
                    printfn "ERR Move piece from %i %i to %i %i: no piece" fromRow fromCol toRow toCol
             | None ->
                printfn "ERR Move piece from %i %i to %i %i: invalid from cell" fromRow fromCol toRow toCol
        | "F" ->
            let (fromRow, fromCol) = (int parts.[2]), (int parts.[3])
            let mutable cell = GetCell newBoard.Cells fromRow fromCol
            let facing = (int parts.[4])
            match Piece.IsValidFacing facing with
            | true ->
                match cell with
                | Some c ->
                    let mutable piece = c.Piece
                    match piece with
                    | Some p -> 
                        piece <- Some (p.ChangeFacing facing)
                        cell <- Some { cell.Value with Piece = piece }
                        newBoard <- { newBoard with Cells = (ReplaceCell newBoard.Cells cell.Value) }
                    | None -> printfn "ERR Change piece facing in %i %i to %i: no piece" fromRow fromCol facing
                | None -> printfn "ERR Change piece facing in %i %i to %i: invalid cell" fromRow fromCol facing
            | false -> printfn "ERR Change piece facing in %i %i to %i: invalid facing" fromRow fromCol facing
        | _ -> raise (new InvalidDataException (sprintf "ERROR: Parsing failure on line %s" line))
    ) definition
    newBoard

let ProcessMovesFromFile fileNameAndPath board =
    try
        let definition = IO.File.ReadAllLines fileNameAndPath
        ProcessMovesFromDefinition board definition
    with
        | :? FileNotFoundException as fnfex -> raise fnfex
        | :? InvalidDataException as idex -> raise idex
        | _ as ex -> raise ex