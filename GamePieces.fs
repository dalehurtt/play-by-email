module GamePieces

open System.Drawing

type PieceID private () =
    let mutable id = 0

    static let instance = PieceID ()

    static member Instance = instance

    member this.GetNextID () =
        id <- id + 1
        id

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

type Piece = {
    ID : int
    Name : string
    Type : PieceType
    Period : Period
    Hits : int
    //State : UnitState
}

type PieceResult = Result<Piece, unit>

let IsValidPiece piecetype period =
    match (period, piecetype) with
    | Period.Ancient, (PieceType.Infantry | PieceType.Archers | PieceType.Skirmishers | PieceType.Cavalry) -> true
    | Period.DarkAges, (PieceType.Infantry | PieceType.Warband | PieceType.Skirmishers | PieceType.Cavalry) -> true
    | Period.Medieval, (PieceType.MenAtArms | PieceType.Levy | PieceType.Crossbowmen | PieceType.Knights) -> true
    | Period.Pike, (PieceType.Infantry | PieceType.Swordsmen | PieceType.Reiters | PieceType.Artillery) -> true
    | Period.Musket, (PieceType.Infantry | PieceType.Skirmishers | PieceType.Artillery | PieceType.Cavalry) -> true
    | Period.Rifle, (PieceType.Infantry | PieceType. Skirmishers | PieceType.Artillery | PieceType.Cavalry) -> true
    | Period.ACW, (PieceType.Infantry | PieceType.Zouaves | PieceType.Artillery | PieceType.Cavalry) -> true
    | Period.Machine, (PieceType.Infantry | PieceType.HeavyInfantry | PieceType.Artillery | PieceType.Cavalry) -> true
    | Period.Modern, (PieceType.Infantry | PieceType.Mortars | PieceType.AntiTankGuns | PieceType.Tanks) -> true
    | _ -> false

let CreatePiece name piecetype period =
    match IsValidPiece piecetype period with
    | true -> 
        Ok {
            ID = PieceID.Instance.GetNextID ()
            Name = name
            Type = piecetype
            Period = period
            Hits = 15
        }
    | false -> Error "invalid period"