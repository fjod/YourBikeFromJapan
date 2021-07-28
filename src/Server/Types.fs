module Server.Types

open Dapper;
open MySql.Data.MySqlClient
open Shared
open Server.Environment
open Database
open Security

let createTokenForUser e =
    let appUser = { email = e }
    encodeJwt appUser


let login  (data : LoginInfo) :LoginResult =
   let user = getUserByEmail data.Email
   match user with
   | Ok _ ->
       let token = createTokenForUser data.Email
       { Result=true; Message=Some "User found"; Token= Some token}
    | Error e ->
       { Result=false; Message=Some e; Token= None }

let register  (data : LoginInfo) :LoginResult =
   let user = getUserByEmail data.Email
   match user with
   | Ok _ ->  { Result=false; Message=Some "User is already registered"; Token=None}
   | Error _ ->
       let r = createUser data.Email data.Password
       match r with
       | Ok _ ->
            let token = createTokenForUser data.Email
            { Result=true; Message=Some "User  registered"; Token=Some token}
       | Error e ->
            { Result=false; Message=Some $"Error on registration {e}"; Token=None}



let validate jwt =
     try
        let _ = decodeJwt jwt
        true
     with _ -> false