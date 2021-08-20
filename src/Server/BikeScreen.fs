module Server.BikeScreen

open Shared
open BikeAPI
//TODO:validateTokens! Validate tokens everyhwere!
let addBikeFun (input:string*BikeRange): BikeRange =
    //for now just return what user added
    //TODO: save it in database for user which we can get from token (fst input)
   snd input


let getBikeModels (input:string*BikeRange): string[] =
     getBikeModelsForRange (snd input)
