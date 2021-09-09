module Server.BikeScreen
open System
open FSharp.Control.Tasks

open Shared
open BikeAPI
open Security
open Database

let addBikeFun (input:string*BikeRange) =

   async {
       let checkToken = validateJwt (fst input)
       match checkToken with
       | Some u ->
                   let! user = getUserByEmail u.Email
                   match user with
                   | Ok u ->
                       let range = snd input
                       let! _ = addBikeToSearch range u
                       return Ok range
                   | Error _ ->
                       return Error NoUserForEmail

       | None ->
              return Error TokenInvalid
   }


let getBikeModels (input:string*BikeRange) =
     getBikeModelsForRange (snd input)





