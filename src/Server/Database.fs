module Server.Database
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
         let! _ = connection.ExecuteScalarAsync($"insert into AuctionData (Manufacturer, Mileage, Img, Year, BikeKey, ScrapedAt)
                                                  values ('{bike.Manufacturer}','{bike.Mileage}','{bike.Image}'),
                                                  '{bike.Year}'),'{bike.Key}'),'{DateTime.Today}');")
                             |> Async.AwaitTask
         ()
    }|> Async.RunSynchronously


let uploadAuctionData (bikes : Bike seq) =
    let z = getSettings
    use connection = new MySqlConnection(z.connectionString)
    let upload = UploadBike connection
    Seq.iter upload bikes


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




