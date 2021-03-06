module PBEM.xTests.Moves

open Xunit
open Setup
open Board
open PBEM

type MoveTests () =
    let boardDefinition = [|
        "B`2`2";
        "C`1`1`0`0"; 
        "C`1`2`0`1`5";
        "C`2`1`3`0";
        "C`2`2`1`1`3"
    |]
    let board = CreateBoardFromDefinition boardDefinition

    [<Fact>]
    let ``Add game piece to a cell`` () =
        let piece = Piece.Create Piece.Side.Red "Red01" Piece.PieceType.Infantry Piece.Period.Ancient 1
        let testBoard = MoveGamePiece piece.Value (0, 0) (1, 1) board
        Assert.True ((GetCell testBoard.Cells 1 1).Value.Piece = piece)

    [<Fact>]
    let ``Move game piece from one cell to another`` () =
        let piece = Piece.Create Piece.Side.Red "Red01" Piece.PieceType.Infantry Piece.Period.Ancient 1
        let startBoard = MoveGamePiece piece.Value (0, 0) (1, 1) board
        let endBoard = MoveGamePiece piece.Value (1, 1) (1, 2) startBoard
        Assert.True ((GetCell endBoard.Cells 1 1).Value.Piece.IsNone)
        Assert.True ((GetCell endBoard.Cells 1 2).Value.Piece = piece)

    [<Theory>]
    [<InlineData(1,3)>]
    [<InlineData(1,5)>]
    [<InlineData(3,7)>]
    let ``Move game piece from one cell to another while changing facing`` (facing, newFacing) =
        let piece = Piece.Create Piece.Side.Red "Red01" Piece.PieceType.Infantry Piece.Period.Ancient facing
        let startBoard = MoveGamePiece piece.Value (0, 0) (1, 1) board
        let newPiece = piece.Value.ChangeFacing newFacing
        let endBoard = MoveGamePiece newPiece (1, 1) (1, 2) startBoard
        Assert.True ((GetCell endBoard.Cells 1 1).Value.Piece.IsNone)
        Assert.True ((GetCell endBoard.Cells 1 2).Value.Piece.Value = newPiece)

    [<Fact>]
    let ``Process setup definition`` () =
        let setupDefinition = [|
            "S`RED`6";
            "P`INF 01`0`0`1`2`1";
            "P`INF 02`0`0`3`2`2";
            "P`INF 03`0`0`1`0`0";
            "P`SKR 01`1`0`1`0`0";
            "P`CAV 01`3`0`1`0`0";
            "P`CAV 02`3`0`1`0`0";
            "S`BLUE`6";
            "P`INF 01`0`0`5`1`1";
            "P`INF 02`0`0`3`1`2";
            "P`INF 03`0`0`5`0`0";
            "P`SKR 01`1`0`5`0`0";
            "P`SKR 02`1`0`5`0`0";
            "P`CAV 01`3`0`5`0`0"
        |]
        let setup = ProcessSetupFromDefinition board setupDefinition
        Assert.True ((GetCell setup.Cells 1 1).Value.Piece.Value.Facing = 5)
        Assert.True ((GetCell setup.Cells 1 2).Value.Piece.Value.Facing = 3)
        Assert.True ((GetCell setup.Cells 2 1).Value.Piece.Value.Facing = 1)
        Assert.True ((GetCell setup.Cells 2 2).Value.Piece.Value.Facing = 3)
        Assert.True (setup.Off.Length = 8)
    