﻿namespace PBEM

open System

module Piece =

    // ======================================== DATA TYPES ========================================

    (*
        PieceType represents the standardized characteristics of a Piece.
    *)
    type PieceType =
        | Unknown = -1
        | Infantry = 0
        | Skirmishers = 1
        | Archers = 2
        | Cavalry = 3
        | Warband = 4
        | Knights = 5
        | Swordsmen = 6
        | Reiters = 7
        | Artillery = 8
        | Zouaves = 9
        | HeavyInfantry = 10
        | Mortars = 11
        | AntiTankGuns = 12
        | Tanks = 13
        | MenAtArms = 14
        | Levy = 15
        | Crossbowmen = 16

    (*
        Period represents the periods (genres) supported by the rules. Period constrains which PieceType
        values a Piece can have and still be valid.
    *)
    type Period =
        | Ancient = 0
        | DarkAges = 1
        | Medieval = 2
        | Pike = 3
        | Musket = 4
        | Rifle = 5
        | ACW = 6
        | Machine = 7
        | Modern = 8

    (*

    *)
    type Side = Red | Blue

    (*
        Piece represents a game piece commonly known in wargames as "units". That word, however, is a
        keyword in F#. Note that the characteristics of these pieces are specific to the rules "One-Hour
        Wargames".

        FUTURE: State represents various effects that can be applied optionally. States such as "shaken",
        "pinned", etc. are possible.
    *)
    type T = {
        ID : Guid
        Name : string
        Type : PieceType
        Period : Period
        Hits : int
        //State : State
        Facing : int
        Side : Side
    }

    // ============================== STATIC FUNCTIONS ==============================

    (*
        Indicates whether a Piece is valid by looking at the assigned PieceType and determining if it is
        valid for indicated Period.
    *)
    let IsValid piecetype period =
        match (period, piecetype) with
        | Period.Ancient, (PieceType.Infantry | PieceType.Archers | PieceType.Skirmishers | PieceType.Cavalry) -> true
        | Period.DarkAges, (PieceType.Infantry | PieceType.Warband | PieceType.Skirmishers | PieceType.Cavalry) -> true
        | Period.Medieval, (PieceType.MenAtArms | PieceType.Levy | PieceType.Crossbowmen | PieceType.Knights) -> true
        | Period.Pike, (PieceType.Infantry | PieceType.Swordsmen | PieceType.Reiters | PieceType.Cavalry) -> true
        | Period.Musket, (PieceType.Infantry | PieceType.Skirmishers | PieceType.Artillery | PieceType.Cavalry) -> true
        | Period.Rifle, (PieceType.Infantry | PieceType. Skirmishers | PieceType.Artillery | PieceType.Cavalry) -> true
        | Period.ACW, (PieceType.Infantry | PieceType.Zouaves | PieceType.Artillery | PieceType.Cavalry) -> true
        | Period.Machine, (PieceType.Infantry | PieceType.HeavyInfantry | PieceType.Artillery | PieceType.Cavalry) -> true
        | Period.Modern, (PieceType.Infantry | PieceType.Mortars | PieceType.AntiTankGuns | PieceType.Tanks) -> true
        | _ -> false

    (*
    Validate that an int falls within acceptable boundaries for facing (which is direction 1, 3, 5, and 7).
        n  : The value to validate.
    *)
    let IsValidFacing n =
        match n with
        | 1|3|5|7 -> true
        | _ -> false
    
    (*
        Returns a Piece given the values passed to create the piece.
    *)
    let Create side name piecetype period facing =
        match IsValid piecetype period with
        | true -> 
            match IsValidFacing facing with
            | true ->
                Some {
                    ID = Guid.NewGuid ()
                    Name = name
                    Type = piecetype
                    Period = period
                    Hits = 15
                    Facing = facing
                    Side = side
                }
            | false -> None
        | false -> None

    // ============================== INSTANCE FUNCTIONS ==============================

    type T with

        (*
        
        *)
        member this.ChangeFacing facing =
            match IsValidFacing facing with
            | true -> { this with Facing = facing }
            | false -> this
