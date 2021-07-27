module Server.Types

open Dapper;
open MySql.Data.MySqlClient
open Shared
open Server.Environment

type UserStorage = {  LoginOrRegister: LoginInfo -> LoginResult }



let login  (data : LoginInfo) :LoginResult =
   //f.LoginOrRegister data
   let z = getSettings
   use connection = new MySqlConnection(z.connectionString)
   //
   let result = connection.Query<DbUser>($"select * from User where email = '{data.Email}' limit 1")

   { Result=true; Message=Some ""; Token="string"}


