module PBEM.xTests.Moves

open System
open Xunit
open GamePieces
open Board
open Setup

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
    let piece = CreatePiece Side.Red "Red01" PieceType.Infantry Period.Ancient 1
    let testBoard = MoveGamePiece piece (0, 0) (1, 1) board
    Assert.True ((GetCell testBoard 1 1).Value.Piece = piece)

[<Fact>]
let ``Move game piece from one cell to another`` () =
    let piece = CreatePiece Side.Red "Red01" PieceType.Infantry Period.Ancient 1
    let startBoard = MoveGamePiece piece (0, 0) (1, 1) board
    let endBoard = MoveGamePiece piece (1, 1) (1, 2) startBoard
    Assert.True ((GetCell endBoard 1 1).Value.Piece.IsNone)
    Assert.True ((GetCell endBoard 1 2).Value.Piece = piece)

[<Theory>]
[<InlineData(1,3)>]
[<InlineData(1,5)>]
[<InlineData(3,7)>]
let ``Move game piece from one cell to another while changing facing`` (facing, newFacing) =
    let piece = CreatePiece Side.Red "Red01" PieceType.Infantry Period.Ancient facing
    let startBoard = MoveGamePiece piece (0, 0) (1, 1) board
    let newPiece = ChangeFacing newFacing piece
    let endBoard = MoveGamePiece newPiece (1, 1) (1, 2) startBoard
    Assert.True ((GetCell endBoard 1 1).Value.Piece.IsNone)
    Assert.True ((GetCell endBoard 1 2).Value.Piece = newPiece)

[<Fact>]
let ``Process setup definition`` () =
    let setupDefinition = [|
        "S`RED`6";
        "P`RED INF 01`0`0`1`2`1";
        "P`RED INF 02`0`0`3`2`2";
        "P`RED INF 03`0`0`1`0`0";
        "P`RED SK 01`1`0`1`0`0";
        "P`RED CAV 01`3`0`1`0`0";
        "P`RED CAV 02`3`0`1`0`0";
        "S`BLUE`6";
        "P`BLUE INF 01`0`0`5`1`1";
        "P`BLUE INF 02`0`0`3`1`2";
        "P`BLUE INF 03`0`0`5`0`0";
        "P`BLUE SK 01`1`0`5`0`0";
        "P`BLUE SK 02`1`0`5`0`0";
        "P`BLUE CAV 01`3`0`5`0`0"
    |]
    let setup = ProcessSetupFromDefinition board setupDefinition
    Assert.True ((GetCell setup 1 1).Value.Piece.Value.Facing = 5)
    Assert.True ((GetCell setup 1 2).Value.Piece.Value.Facing = 3)
    Assert.True ((GetCell setup 2 1).Value.Piece.Value.Facing = 1)
    Assert.True ((GetCell setup 2 2).Value.Piece.Value.Facing = 3)