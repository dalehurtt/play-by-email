module Setup

open System.IO
open PBEM
open Board
open System

let ProcessSetupFromDefinition board definition =
    let mutable currentSide = Piece.Side.Red
    let mutable newBoard = board
    Array.iter (fun (line : string) ->
        let parts = line.Split ([|'`'|])
        match parts.[0] with
        | "S" ->
            match parts.[1] with
            | "RED" -> currentSide <- Piece.Side.Red
            | "BLUE" -> currentSide <- Piece.Side.Blue
            | _ -> raise (new InvalidDataException (sprintf "ERROR: Parsing failure on line %s" line))
        | "P" ->
            let pieceType = Enum.Parse (typedefof<Piece.PieceType>, parts.[2]) :?> Piece.PieceType
            let period = Enum.Parse (typedefof<Piece.Period>, parts.[3]) :?> Piece.Period 
            let piece = Piece.Create currentSide parts.[1] pieceType period (int parts.[4])
            newBoard <- MoveGamePiece piece.Value (0, 0) ((int parts.[5]), (int parts.[6])) newBoard
        | _ -> raise (new InvalidDataException (sprintf "ERROR: Parsing failure on line %s" line))
    ) definition
    newBoard

let ProcessSetupFromFile fileNameAndPath board =
    try
        let definition = IO.File.ReadAllLines fileNameAndPath
        ProcessSetupFromDefinition board definition
    with
        | :? FileNotFoundException as fnfex -> raise fnfex
        | :? InvalidDataException as idex -> raise idex
        | _ as ex -> raise ex
