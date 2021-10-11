module Server.WelcomeScreen


open Server.Types
open Shared
open Database
open Security

let createTokenForUser e =
    let appUser : UserInfo = { Email = e }
    encodeJwt appUser


let login  (data : LoginInfo) =
   async {
       let! user = getUserByEmail data.Email
       match user with
       | Ok _ ->
           let token = createTokenForUser data.Email
           return {  Message=Some "User found"; Token= Some token}
        | Error e ->
           return {  Message=Some "Auth Error"; Token= None }
   }
let register  (data : LoginInfo)  =
   async {
       let! user = getUserByEmail data.Email
       match user with
       | Ok _ ->  return { Message=Some "User is already registered"; Token=None}
       | Error _ ->
           let! r = createUser data.Email data.Password
           match r with
           | Ok _ ->
                let token = createTokenForUser data.Email
                return {  Message=Some "User  registered"; Token=Some token}
           | Error e ->
                return {  Message=Some $"Error on registration {e}"; Token=None}
   }

let validate jwt =
     try
        let _ = decodeJwt jwt //user is encoded in token
        true
     with _ -> false