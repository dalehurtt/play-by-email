module Setup

open System.IO
open GamePieces
open Board
open System

let ProcessSetupFromDefinition (board : Board) (definition : string[]) : Board =
    let mutable currentSide = None
    let mutable newBoard = board
    Array.iter (fun (line : string) ->
        let parts = line.Split ([|'`'|])
        match parts.[0] with
        | "S" ->
            match parts.[1] with
            | "RED" -> currentSide <- Some Side.Red
            | "BLUE" -> currentSide <- Some Side.Blue
            | _ -> raise (new InvalidDataException (sprintf "ERROR: Parsing failure on line %s" line))
        | "P" ->
            let pieceType = Enum.Parse (typedefof<PieceType>, parts.[2]) :?> PieceType
            let period = Enum.Parse (typedefof<Period>, parts.[3]) :?> Period 
            let piece = CreatePiece currentSide.Value parts.[1] pieceType period (int parts.[4])
            newBoard <- MoveGamePiece piece (0, 0) ((int parts.[5]), (int parts.[6])) newBoard
        | _ -> raise (new InvalidDataException (sprintf "ERROR: Parsing failure on line %s" line))
    ) definition
    newBoard