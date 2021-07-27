module Server.Database
open System
open Dapper;
open MySql.Data.MySqlClient
open Shared
open Server.Environment

let getUserByEmail(email:string) =
   async {
       let z = getSettings
       use connection = new MySqlConnection(z.connectionString)

       let (|IsNull|NotNull|) (input:Object) = if Object.ReferenceEquals(input,null) then IsNull else NotNull
       let! result = connection.QueryFirstOrDefaultAsync<DbUser>($"select * from User where email = '{email}' limit 1") |> Async.AwaitTask
       let convert input =
           match input with
           | IsNull -> Error "user not found"
           | NotNull -> Ok input
       return convert result

   } |> Async.RunSynchronously
