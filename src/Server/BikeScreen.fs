module Server.BikeScreen

open Shared
open BikeAPI
open Security
open Database
//TODO:validateTokens! Validate tokens everywhere!
let addBikeFun (input:string*BikeRange): Result<BikeRange,AuthError> =
    //for now just return what user added
    //TODO: save it in database for user which we can get from token (fst input)
   let checkToken = validateJwt (fst input)
   match checkToken with
   | Some u ->
       let user = getUserByEmail u.Email
       match user with
       | Ok u ->
           let range = snd input
           addBikeToSearch range u |> Async.RunSynchronously
           Ok range
       | Error _ ->
           Error NoUserForEmail
   | None ->
           Error TokenInvalid


let getBikeModels (input:string*BikeRange): string[] =
     getBikeModelsForRange (snd input)
