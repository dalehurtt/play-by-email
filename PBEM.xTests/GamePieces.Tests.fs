module PBEM.xTests.GamePieces

open Xunit
open GamePieces

[<Theory>]
[<InlineData (PieceType.Infantry, Period.Ancient)>]
[<InlineData (PieceType.Warband, Period.DarkAges)>]
[<InlineData (PieceType.MenAtArms, Period.Medieval)>]
[<InlineData (PieceType.Reiters, Period.Pike)>]
[<InlineData (PieceType.Skirmishers, Period.Musket)>]
[<InlineData (PieceType.Artillery, Period.Rifle)>]
[<InlineData (PieceType.Zouaves, Period.ACW)>]
[<InlineData (PieceType.HeavyInfantry, Period.Machine)>]
[<InlineData (PieceType.Tanks, Period.Modern)>]
let ``Tests for valid game pieces by period`` (pieceType, period) =
    let expected = true
    let actual = IsValidPiece pieceType period
    Assert.Equal (expected, actual)

[<Theory>]
[<InlineData (PieceType.Warband, Period.Ancient)>]
[<InlineData (PieceType.Archers, Period.DarkAges)>]
[<InlineData (PieceType.Infantry, Period.Medieval)>]
[<InlineData (PieceType.Artillery, Period.Pike)>]
[<InlineData (PieceType.Reiters, Period.Musket)>]
[<InlineData (PieceType.Zouaves, Period.Rifle)>]
[<InlineData (PieceType.Skirmishers, Period.ACW)>]
[<InlineData (PieceType.Tanks, Period.Machine)>]
[<InlineData (PieceType.HeavyInfantry, Period.Modern)>]
let ``Tests for invalid game pieces by period`` (pieceType, period) =
    let expected = false
    let actual = IsValidPiece pieceType period
    Assert.Equal (expected, actual)

[<Theory>]
[<InlineData (PieceType.Warband, Period.Ancient)>]
[<InlineData (PieceType.Archers, Period.DarkAges)>]
[<InlineData (PieceType.Infantry, Period.Medieval)>]
[<InlineData (PieceType.Archers, Period.Pike)>]
[<InlineData (PieceType.Reiters, Period.Musket)>]
[<InlineData (PieceType.Zouaves, Period.Rifle)>]
[<InlineData (PieceType.Skirmishers, Period.ACW)>]
[<InlineData (PieceType.Tanks, Period.Machine)>]
[<InlineData (PieceType.HeavyInfantry, Period.Modern)>]
let ``Creation tests for invalid game pieces by period`` (pieceType, period) =
    let actual = CreatePiece Side.Red "A" pieceType period  1
    match actual with
    | Some piece -> Assert.False (true)
    | None -> Assert.True (true)

[<Theory>]
[<InlineData (1)>]
[<InlineData (3)>]
[<InlineData (5)>]
[<InlineData (7)>]
let ``Create a game piece facing a valid direction`` facing =
    let actual = CreatePiece Side.Red "A" PieceType.Infantry Period.Ancient facing
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
    let actual = CreatePiece Side.Red "A" PieceType.Infantry Period.Ancient facing
    Assert.True (actual.IsNone)

