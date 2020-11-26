module PBEM.xTests.Moves

open System
open Xunit
open GamePieces
open Board

let boardDefinition = [|
    "B`2`2";
    "C`1`1`0`0`0"; 
    "C`1`2`0`1`5";
    "C`2`1`3`0`0";
    "C`2`2`1`1`3"
|]
let board = CreateBoardFromDefinition boardDefinition

[<Fact>]
let ``Add game piece to a cell`` () =
    let piece = CreatePiece "Red01" PieceType.Infantry Period.Ancient
    let testBoard = MoveGamePiece piece (0, 0) (1, 1) board
    Assert.True ((GetCell testBoard 1 1).Value.Piece = piece)

[<Fact>]
let ``Move game piece from one cell to another`` () =
    let piece = CreatePiece "Red01" PieceType.Infantry Period.Ancient
    let startBoard = MoveGamePiece piece (0, 0) (1, 1) board
    let endBoard = MoveGamePiece piece (1, 1) (1, 2) startBoard
    Assert.True ((GetCell endBoard 1 1).Value.Piece.IsNone)
    Assert.True ((GetCell endBoard 1 2).Value.Piece = piece)
