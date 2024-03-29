﻿module Server.Database
open System
open System.Collections.Generic
open System.Text
open Dapper;
open Fake.Core
open MySql.Data.MySqlClient
open Server.Types.DbTypes
open Shared
open Server.Environment
open Server.Security
open Types
open Shared.NullHelper

//docker run --name JapanBike2 -v c:/tmp/mysql:/var/lib/mysql -p 3306:3306 -e MYSQL_ROOT_PASSWORD=312312 -d mysql:latest
let getUserByEmail(email:string) =
   async {
       use connection = new MySqlConnection(getSettings.connectionString)
       let! result = connection.QueryFirstOrDefaultAsync<DbUser>($"select * from User where email = '{email}' limit 1") |> Async.AwaitTask
       return convert result
   }

let createUser(email:string) (inputPassword:string)=
    let salt = createRandomKey()
    let password = utf8Bytes inputPassword
    let saltyPassword = Array.concat [ salt; password ]
    let passwordHash = sha256Hash saltyPassword
    let passForDb = base64 passwordHash
    let saltForDb = base64 salt
    async {
           try
               use connection = new MySqlConnection(getSettings.connectionString)
               let! _ = connection.QueryFirstOrDefaultAsync<DbUser>($"insert into User (email, password, salt) values ('{email}','{passForDb}','{saltForDb}');")
                             |> Async.AwaitTask
               return Ok ""
            with
             | :? Exception as e  ->
                                     printfn "%s" e.Message
                                     return Error e.Message
       }

let UploadBike (connection:MySqlConnection) (bike:Bike) =
    async{
         let m = BikeRangeHelper.ManufacturerToString bike.Manufacturer
         let! _ = connection.ExecuteScalarAsync($"insert into AuctionData (Manufacturer, Mileage, Img, Year, BikeKey, ScrapedAt, Model)
                                                  values ('{m}','{bike.Mileage}','{bike.Image}'),
                                                  '{bike.Year}'),'{bike.Key}'),'{DateTime.Today}', {bike.Model});")
                             |> Async.AwaitTask
         ()
    }

let GetBikesKeys (connection : MySqlConnection) =
   async {
        let! result = connection.QueryAsync<string>("select BikeKey from AuctionData") |> Async.AwaitTask
        return result
    }

let uploadAuctionData (bikes : Bike seq) =
    async {
        use connection = new MySqlConnection(getSettings.connectionString)
        let upload = UploadBike connection
        let! presentBikes = GetBikesKeys connection
        let pb = presentBikes.AsList()
        bikes |> Seq.where (fun b -> not (pb.Contains(b.Key))) |> Seq.iter (fun b -> upload b |> ignore)
    }

let getBikeIdFromRange (bike:BikeRange) (connection : MySqlConnection)=
    async {
        let m = BikeRangeHelper.ManufacturerToString bike.Maker
        let goodData = dict [ "StartYear", box  bike.StartYear; "EndYear",box  bike.EndYear; "Maker" , box m; "Model", box bike.Model]
        let! result = connection.QueryAsync<int>($"select id from BikeModel where Year >= @StartYear
                                                 and Year <= @EndYear and Model = @Model and Maker = @Maker",goodData) |> Async.AwaitTask
        return result
    }

let getModelsForRange(range:BikeRange)=
    async {
        try
            use connection = new MySqlConnection(getSettings.connectionString)
            let m = BikeRangeHelper.ManufacturerToString range.Maker
            let! result = connection.QueryAsync<string>($"select Model from BikeModel where Year >= {range.StartYear}
                                                     and Year <= {range.EndYear} and Maker = '{m}'") |> Async.AwaitTask
            return result
         with
             | :? Exception as e  ->
                                     printfn "%s" e.Message
                                     return  List<string>() :> IEnumerable<string>
    }


let addBikeToSearch (bike: BikeRange) (user : DbUser) =
   async{
         try
             use connection = new MySqlConnection(getSettings.connectionString)
             let! id = getBikeIdFromRange bike connection
             let asList = id.AsList()
             if (asList.Count > 0) then
                 let bikeId = asList.[0]
                 let goodData = dict ["UserId", box user.id; "BikeId", box bikeId; "StartYear", box  bike.StartYear; "EndYear",box  bike.EndYear ]
                 let! _ = connection.ExecuteScalarAsync($"insert into BikesForUser (User, BikeModel, StartYear, EndYear)
                                                          values (@UserId, @BikeId , @StartYear, @EndYear);",goodData)
                                     |> Async.AwaitTask
                 ()
          with
             | :? Exception as e  ->
                                     printfn "%s" e.Message
                                     ()
    }

let test = "select * from test"

let getUserBikes (user:DbUser) =
    async {
         try
             use connection = new MySqlConnection(getSettings.connectionString)
             let goodData = dict ["UserId", box user.id ]
             let! userBikes = connection.QueryAsync<DbBikeRange>("select BM.Maker as Maker, BM.Model as Model, StartYear, EndYear  from BikesForUser
                                                join BikeModel BM on BM.id = BikesForUser.BikeModel
                                                join User U on U.id = BikesForUser.User
                                                where U.id = @UserId;",goodData)
                                     |> Async.AwaitTask
             return userBikes.AsList().ToArray() |> Array.map ConvertToBikeRange  |> Array.choose id
         with
             | :? Exception as e  ->
                                     printfn "%s" e.Message
                                     return [||]
    }


let getBikesForUsers()=
    async{
         use connection = new MySqlConnection(getSettings.connectionString)
         let! result = connection.QueryAsync<DbTypes.DBBikeToSearch>("select Min(BU.StartYear) as MinYear, Max(bu.EndYear) as MaxYear, BM.Maker from BikesForUser BU join BikeModel BM on BM.id = BU.BikeModel group by  bm.Maker")
                       |> Async.AwaitTask
         return result
    }

let GetAllBikeModels() =
    async {
         try
             use connection = new MySqlConnection(getSettings.connectionString)
             connection.Open ()
             let! result = connection.QueryAsync<DbTypes.DbBikeModel>("select Maker,Model,Year from BikeModel")
                           |> Async.AwaitTask
             connection.Close ()
             return result
          with
          | :? Exception as e  ->
                                     printfn "%s" e.Message
                                     return  List<DbTypes.DbBikeModel>() :> IEnumerable<DbTypes.DbBikeModel>
    }
let fillBikeModelTable(vals: DbTypes.DbBikeModel seq) =
   async {
       let sb =  StringBuilder("insert into BikeModel (Maker, Model, Year) values ")
       let initialLength = sb.Length
       Console.WriteLine " started upload to db"

       let! existingModels = GetAllBikeModels()
       let listOfExistingModels = existingModels.AsList()
       vals |> Seq.where (fun v -> not (listOfExistingModels.Contains(v)))
            |> Seq.iter (fun bike -> sb.Append($" ('{bike.Maker}','{bike.Model.Replace(''',' ')}','{bike.Year}'),") |> ignore)
       if (initialLength + 5 > sb.Length) then
           Console.WriteLine " nothing to upload"
           return () //nothing to upload
         else
           //try catch here
           try
               use connection = new MySqlConnection(getSettings.connectionString)
               connection.Open ()
               let! _ = connection.ExecuteScalarAsync(sb.Remove(sb.Length-1,1).Append(";").ToString())    |> Async.AwaitTask
               connection.Close ()
               Console.WriteLine " finished upload to db"
               return ()
           with
            | :? Exception as e  ->
                                     printfn "%s" e.Message
                                     return  ()
   }

let bikesKeys =
    async {

             use connection = new MySqlConnection(getSettings.connectionString)
             connection.Open ()
             let! result = connection.QueryAsync<string>("select BikeKey from AuctionData")
                           |> Async.AwaitTask
             connection.Close ()
             return result
    }

let insertWithMemo(bikes : Bike seq) =
    async{
        try
            let sb =  StringBuilder("insert into AuctionData (Manufacturer, Mileage, Img, Year, Model, BikeKey, ScrapedAt) values ")
            let! list = bikesKeys
            let asList = list.AsList()
            let formattedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            bikes |> Seq.filter (fun b -> asList.Contains(b.Key) |> not) |>
                     Seq.iter   (fun b -> sb.Append($" ('{b.Manufacturer}','{b.Mileage}','{b.Image}','{b.Year}','{b.Model.Replace(''',' ')}','{b.Key}','{formattedTime}'),") |> ignore)

            use connection = new MySqlConnection(getSettings.connectionString)
            connection.Open ()
            let! _ = connection.ExecuteScalarAsync(sb.Remove(sb.Length-1,1).Append(";").ToString())    |> Async.AwaitTask
            connection.Close ()
            Console.WriteLine " finished upload bikes to db"
            ()
        with
            | :? Exception as e  ->
                                     printfn "%s" e.Message
                                     return  ()
    }

let auctDataForRange(range :BikeRange) =
    async {
         try
             use connection = new MySqlConnection(getSettings.connectionString)
             let goodData = dict [
                 "model", box range.Model
                 "EndYear", box range.EndYear
                 "StartYear", box range.StartYear
             ]
             let! userBikes = connection.QueryAsync<AuctionData>("select Model,       Year,       BikeKey,       Img,
                            Mileage,       ScrapedAt,  Manufacturer from AuctionData where Model = @model and Year <= @EndYear and Year >= @StartYear;",goodData)
                                     |> Async.AwaitTask
             return userBikes.AsList().ToArray()
         with
             | :? Exception as e  ->
                                     printfn "%s" e.Message
                                     return [||]
    }

    //таблица user id : auct data id
    //её используем для отправки емейлов по новым байкам