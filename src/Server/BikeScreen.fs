module Server.BikeScreen

open Shared

let addBikeFun (input:string*BikeRange): string =
    //for now just return what user added
    //TODO: save it in database for user which we can get from token (fst input)
    let bike = snd input
    bike.Model

let getBikeModels (input:string*BikeRange): string[] =

    ()