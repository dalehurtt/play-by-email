module PBEM.Tests.GamePieces

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
let ``Tests for valid game pieces by period`` (gamepiece, period) =
    let expected = true
    let actual = IsValidPiece gamepiece period
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
let ``Tests for invalid game pieces by period`` (gamepiece, period) =
    let expected = false
    let actual = IsValidPiece gamepiece period
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
let ``Creation tests for invalid game pieces by period`` (gamepiece, period) =
    let actual = CreatePiece "A" gamepiece period
    match actual with
    | Ok piece -> Assert.False (true)
    | Error message -> Assert.True (true)
