﻿module Server.Database
open System
open Dapper;
open MySql.Data.MySqlClient
open Shared
open Server.Environment
open Server.Security

open Shared.NullHelper

let getUserByEmail(email:string) =
   async {
       let z = getSettings
       use connection = new MySqlConnection(z.connectionString)
       let! result = connection.QueryFirstOrDefaultAsync<DbUser>($"select * from User where email = '{email}' limit 1") |> Async.AwaitTask

       return convert result
   } |> Async.RunSynchronously

let createUser(email:string) (inputPassword:string)=
    let salt = createRandomKey()
    let password = utf8Bytes inputPassword
    let saltyPassword = Array.concat [ salt; password ]
    let passwordHash = sha256Hash saltyPassword
    let passForDb = base64 passwordHash
    let saltForDb = base64 salt
    async {
           let z = getSettings
           use connection = new MySqlConnection(z.connectionString)
           let! result = connection.QueryFirstOrDefaultAsync<DbUser>($"insert into User (email, password, salt) values ('{email}','{passForDb}',{saltForDb});")
                         |> Async.AwaitTask

           return convert result
       } |> Async.RunSynchronously