module Server.BikeScreen
open System
open FSharp.Control.Tasks

open FsToolkit.ErrorHandling.Operator.AsyncResult
open Shared
open BikeAPI
open Security
open Database
open FsToolkit.ErrorHandling

let addBikeFun (input:string*BikeRange) =

   async {
       let checkToken = validateJwt (fst input)
       match checkToken with
       | Ok u ->
                   let! user = getUserByEmail u.Email
                   match user with
                   | Ok u ->
                       let range = snd input
                       let! _ = addBikeToSearch range u
                       return Ok range
                   | Error _ ->
                       return Error NoUserForEmail

       | Error _ ->
              return Error TokenInvalid
   }

let getBikeModels (input:string*BikeRange) =
     async {
       let checkToken = validateJwt (fst input)
       match checkToken with
       | Ok u ->
             let! r =  getBikeModelsForRange (snd input)
             return Ok r
       | Error _ ->
              return Error TokenInvalid
   }

let getUserBikesFun (input:string) =
    async {
           let checkToken = validateJwt input
           match checkToken with
           | Ok u ->
                       let! user = getUserByEmail u.Email
                       match user with
                       | Ok u ->
                           let! bikes = getUserBikes u
                           return Ok bikes
                       | Error _ ->
                           return Error NoUserForEmail

           | Error _ ->
                  return Error TokenInvalid
       }

let test(input:string*BikeRange)=
    asyncResult {
        let! token =  (fst input) |> validateJwt |> Result.map (fun r -> r.Email)
        let! _ = token |> getUserByEmail
        return! snd input |> auctDataForRange
        }

let getAuctData(input:string*BikeRange) =
     async {
       let checkToken = validateJwt (fst input)
       match checkToken with
       | Ok  u ->
                   let! user = getUserByEmail u.Email
                   match user with
                   | Ok u ->
                       let range = snd input
                       let! data = auctDataForRange range
                       return Ok data
                   | Error _ ->
                       return Error NoUserForEmail

       | Error _ ->
              return Error TokenInvalid
   }
