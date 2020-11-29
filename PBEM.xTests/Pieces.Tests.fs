module PBEM.xTests.Pieces

open Xunit
open PBEM

[<Theory>]
[<InlineData (Piece.PieceType.Infantry, Piece.Period.Ancient)>]
[<InlineData (Piece.PieceType.Warband, Piece.Period.DarkAges)>]
[<InlineData (Piece.PieceType.MenAtArms, Piece.Period.Medieval)>]
[<InlineData (Piece.PieceType.Reiters, Piece.Period.Pike)>]
[<InlineData (Piece.PieceType.Skirmishers, Piece.Period.Musket)>]
[<InlineData (Piece.PieceType.Artillery, Piece.Period.Rifle)>]
[<InlineData (Piece.PieceType.Zouaves, Piece.Period.ACW)>]
[<InlineData (Piece.PieceType.HeavyInfantry, Piece.Period.Machine)>]
[<InlineData (Piece.PieceType.Tanks, Piece.Period.Modern)>]
let ``Tests for valid game pieces by period`` (pieceType, period) =
    let expected = true
    let actual = Piece.IsValid pieceType period
    Assert.Equal (expected, actual)

[<Theory>]
[<InlineData (Piece.PieceType.Warband, Piece.Period.Ancient)>]
[<InlineData (Piece.PieceType.Archers, Piece.Period.DarkAges)>]
[<InlineData (Piece.PieceType.Infantry, Piece.Period.Medieval)>]
[<InlineData (Piece.PieceType.Artillery, Piece.Period.Pike)>]
[<InlineData (Piece.PieceType.Reiters, Piece.Period.Musket)>]
[<InlineData (Piece.PieceType.Zouaves, Piece.Period.Rifle)>]
[<InlineData (Piece.PieceType.Skirmishers, Piece.Period.ACW)>]
[<InlineData (Piece.PieceType.Tanks, Piece.Period.Machine)>]
[<InlineData (Piece.PieceType.HeavyInfantry, Piece.Period.Modern)>]
let ``Tests for invalid game pieces by period`` (pieceType, period) =
    let expected = false
    let actual = Piece.IsValid pieceType period
    Assert.Equal (expected, actual)

[<Theory>]
[<InlineData (Piece.PieceType.Warband, Piece.Period.Ancient)>]
[<InlineData (Piece.PieceType.Archers, Piece.Period.DarkAges)>]
[<InlineData (Piece.PieceType.Infantry, Piece.Period.Medieval)>]
[<InlineData (Piece.PieceType.Archers, Piece.Period.Pike)>]
[<InlineData (Piece.PieceType.Reiters, Piece.Period.Musket)>]
[<InlineData (Piece.PieceType.Zouaves, Piece.Period.Rifle)>]
[<InlineData (Piece.PieceType.Skirmishers, Piece.Period.ACW)>]
[<InlineData (Piece.PieceType.Tanks, Piece.Period.Machine)>]
[<InlineData (Piece.PieceType.HeavyInfantry, Piece.Period.Modern)>]
let ``Creation tests for invalid game pieces by period`` (pieceType, period) =
    let actual = Piece.Create Piece.Side.Red "A" pieceType period  1
    match actual with
    | Some piece -> Assert.False (true)
    | None -> Assert.True (true)

[<Theory>]
[<InlineData (1)>]
[<InlineData (3)>]
[<InlineData (5)>]
[<InlineData (7)>]
let ``Create a game piece facing a valid direction`` facing =
    let actual = Piece.Create Piece.Side.Red "A" Piece.PieceType.Infantry Piece.Period.Ancient facing
    Assert.True (actual.IsSome)
    Assert.True (actual.Value.Facing = facing)

[<Theory>]
[<InlineData (0)>]
[<InlineData (2)>]
[<InlineData (4)>]
[<InlineData (6)>]
[<InlineData (8)>]
[<InlineData (9)>]
let ``Create a game piece facing an invalid direction`` facing =
    let actual = Piece.Create Piece.Side.Red "A" Piece.PieceType.Infantry Piece.Period.Ancient facing
    Assert.True (actual.IsNone)

