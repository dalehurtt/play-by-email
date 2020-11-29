module Pure

// ============================== MOVE TO NO DEPENDENCIES ==============================

(*

*)
let FacingToString facing =
    match facing with
    | 1 -> "N"
    | 3 -> "E"
    | 5 -> "S"
    | 7 -> "W"
    | _ -> "?"
