// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open MySql.Data
open System.Data
open System.Net
open System.Net.Http
open System.Text.Json
open System.Text.Json.Serialization
open Dapper
open MySql.Data.MySqlClient;
open Server.Environment
open Shared;

[<CLIMutable>]
type Id = {
    [<JsonPropertyName("$oid")>]
    Oid:string
}

[<CLIMutable>]
type MetaData = {ranking_status : string}

[<CLIMutable>]
type Bike = {
       metadata : MetaData;
      _id : Id;
         auction : string;
          bid_number :int;
          category : Object;
          condition_engine :string;
          condition_exterior : string;
          condition_frame : string;
          condition_front : string;
          condition_parts : string;
          condition_rear :string;
          date : string;
          engine_volume :int;
          final_bid :int;
          final_bid_rub : int;
          frame_number :string;
          key :string;
          lane :Object;
          manufacturer :string;
          mileage :int;
          model :string;
          model_alise :string;
          preview_image :string;
          rank: string;
          result :string;
          starting_bid : int;
          starting_bid_rub : int;
          state : Object;
          year :int;
}
[<CLIMutable>]
type BikeModel = {
    id : int
    Maker : string
    Model : string
    Year : int
}

[<CLIMutable>]
type Root = {bikes : Bike seq  }

// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom

let GetConnection()  =
    new MySqlConnection("Server=localhost;Database=Bikes;Uid=root;Pwd=312312;") //:> IDbConnection


[<EntryPoint>]
let main argv =
    let () = printfn "hello"


    let request = "https://projapan.ru/bikes?mileage_min=0&mileage_max=100&year_min=2009&year_max=2021&rank_min=0&rank_max=5&auction%5B%5D=auc&auction%5B%5D=bds&auction%5B%5D=jba&auction%5B%5D=arai&date=all&volume=750&manufacturer=H&model="
    let client = new HttpClient()
    let ins (data:BikeModel) (connection:IDbConnection ) =
        connection.Query<BikeModel>($"insert into BikeModel (Maker, Model, Year) values ('{data.Maker}','{data.Model}',{data.Year});") |> ignore
        ()

    async {

//        use conn = new MySqlConnection("Server=localhost;Port=3306;Database=Bikes;Uid=root;Pwd=312312;")
//        conn.Open ()
//        let! result = conn.QueryFirstOrDefaultAsync<DbUser>($"insert into User (email, password, salt) values ('5','O1r/B02i+Fkf49oGke68sFOWdKIH7jd+LpmNIEhHPF8=','9qRanZ8wuODzMuz8t1pTdU2Ydqe3bxz65sp4DziitrI=');")
//                         |> Async.AwaitTask
//        printfn "Hello world %s" result.email
//        let! result = conn.QueryFirstOrDefaultAsync<DbUser>($"select * from User where email = '1' limit 1") |> Async.AwaitTask
//        printfn "Hello world %s" result.email
        let message = new HttpRequestMessage(HttpMethod.Get, request)
        message.Headers.Add ( "Host", "projapan.ru" )
        message.Headers.Add ( HttpRequestHeader.ContentType.ToString(), "application/json; charset=utf-8" )
        message.Headers.Add ( "Accept", "*/*" )
        message.Headers.Add ( "Connection", "keep-alive" )

        let! response = client.SendAsync(message) |> Async.AwaitTask
        response.EnsureSuccessStatusCode () |> ignore
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        let json = JsonSerializer.Deserialize<Root> content
        let conv = Seq.map (fun b ->  {Maker = "Honda"
                                       Model = b.model
                                       Year = b.year
                                       id = 0}) json.bikes


        use conn = new MySqlConnection("Server=localhost;Port=6603;Database=Bikes;Uid=root;Pwd=312312;")
        conn.Open ()
        Seq.iter (fun b -> ins  b conn) conv
        conn.Close ()
        ()
    } |>  Async.RunSynchronously


    let message = from "F#" // Call the function
    printfn "Hello world %s" message
    0 // return an integer exit code