type Int1To8 = 
    | Int1To8 of int
    static member Create (n) =
        match n with
        | n when n >= 1 && n <= 8 -> Int1To8 n
        | _ -> failwith "out of range 1..8"

type R = {
    F : Int1To8
    T : Int1To8
}

let r1 : R = { F = Int1To8.Create 1; T = Int1To8.Create 8 }
let r2 : R = { F = Int1To8.Create 1; T = Int1To8.Create 9 }
let r3 = { F = Int1To8.Create 1; T = Int1To8.Create 8 }

// An integer restricted to the values between 1 and 8.
//type Int1To8 = Int1To8 of int

// Utility to create a Int1To8 type.
let CreateInt1To8 n =
    match n with
    | n when n >= 1 && n <= 8 -> Int1To8 n
    | _ -> failwith "out of range 1..8"

type Period =
    | Ancient = 0
    | DarkAges = 1

type PieceType =
    | Unknown = -1
    | Infantry = 0
    | Skirmishers = 1
    | Archers = 2
    | Cavalry = 3
    | Warband = 4

let IsPieceTypeValid piecetype period =
    match (period, piecetype) with
    | Period.Ancient, (PieceType.Infantry | PieceType.Archers | PieceType.Skirmishers | PieceType.Cavalry) -> true
    | Period.DarkAges, (PieceType.Infantry | PieceType.Warband | PieceType.Skirmishers | PieceType.Cavalry) -> true
    | _ -> false

