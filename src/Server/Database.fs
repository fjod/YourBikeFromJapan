﻿module Server.Database
open System
open Dapper;
open MySql.Data.MySqlClient
open Shared
open Server.Environment
open Server.Security

open Shared.NullHelper

//docker run --name JapanBike2 -v c:/tmp/mysql:/var/lib/mysql -p 3306:3306 -e MYSQL_ROOT_PASSWORD=312312 -d mysql:latest
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
           try

               let z = getSettings
               use connection = new MySqlConnection(z.connectionString)
               let! _ = connection.QueryFirstOrDefaultAsync<DbUser>($"insert into User (email, password, salt) values ('{email}','{passForDb}','{saltForDb}');")
                             |> Async.AwaitTask
               return Ok ""
            with
             | :? Exception as e  ->
                                     printfn "%s" e.Message
                                     return Error e.Message
       } |> Async.RunSynchronously

let UploadBike (connection:MySqlConnection) (bike:Bike) =
    async{
         let m = BikeRangeHelper.BikeRangeToString bike.Manufacturer
         let! _ = connection.ExecuteScalarAsync($"insert into AuctionData (Manufacturer, Mileage, Img, Year, BikeKey, ScrapedAt)
                                                  values ('{m}','{bike.Mileage}','{bike.Image}'),
                                                  '{bike.Year}'),'{bike.Key}'),'{DateTime.Today}');")
                             |> Async.AwaitTask
         ()
    }

let GetBikesKeys (connection : MySqlConnection) =
   async {
        let! result = connection.QueryAsync<string>($"select BikeKey from AuctionData") |> Async.AwaitTask
        return result
    }

let uploadAuctionData (bikes : Bike seq) =
    async {
        let z = getSettings
        use connection = new MySqlConnection(z.connectionString)
        let upload = UploadBike connection
        let! presentBikes = GetBikesKeys connection
        let pb = presentBikes.AsList()
        bikes |> Seq.where (fun b -> not (pb.Contains(b.Key))) |> Seq.iter (fun b -> upload b |> ignore)
    }

let getBikeIdFromRange (bike:BikeRange) (connection : MySqlConnection)=
    async {
        let m = BikeRangeHelper.BikeRangeToString bike.Maker
        let! result = connection.QueryAsync<int>($"select id from BikeModel where Year >= {bike.StartYear}
                                                 and Year <= {bike.EndYear} and Model == {bike.Model} and Maker == {m} and Model == {bike.Model}") |> Async.AwaitTask
        return result
    }


let addBikeToSearch (bike: BikeRange) (user : DbUser) =
   async{
         let z = getSettings
         use connection = new MySqlConnection(z.connectionString)

         let! id = getBikeIdFromRange bike connection
         let asList = id.AsList()
         if (asList.Count > 0) then
             let userId = asList.[0]
             let! _ = connection.ExecuteScalarAsync($"insert into BikesForUser (User, BikeModel, StartYear, EndYear)
                                                      values ('{user.id}','{userId}','{bike.StartYear}','{bike.EndYear}');")
                                 |> Async.AwaitTask
             ()
    }//|> Async.RunSynchronously

//create table if not exists BikeModel
//(
//	id int auto_increment
//		primary key,
//	Maker varchar(10) not null,
//	Model varchar(30) not null,
//	Year smallint null
//);
//
//create index BikeModel__index
//	on BikeModel (Maker);

//create table if not exists User
//(
//	id int auto_increment
//		primary key,
//	email varchar(50) not null,
//	password varchar(64) not null,
//	salt varchar(64) not null,
//	constraint User_email_uindex
//		unique (email)
//);
//create table BikesForUser
//(
//    User      int not null,
//    BikeModel int not null,
//    StartYear int not null,
//    EndYear   int not null,
//    constraint BikesForUser_BikeModel_id_fk
//        foreign key (BikeModel) references BikeModel (id),
//    constraint BikesForUser_User_id_fk
//        foreign key (User) references User (id)
//);
//create table AuctionData
//(
//	id int auto_increment,
//	Manufacturer varchar(15) not null,
//	Mileage int not null,
//	Img varchar(100) null,
//	Year int not null,
//	BikeKey varchar(20) not null,
//   ScrapedAt    datetime     not null,
//	constraint AuctionData_pk
//		primary key (id)
//);




