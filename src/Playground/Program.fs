// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO
open System.IO.Compression
open System.Net
open System.Net.Http
open System.Text.Json
open System.Text.Json.Serialization

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
          year :Object;
}

[<CLIMutable>]
type Root = {bikes : Bike seq  }

// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom



[<EntryPoint>]
let main argv =
    let request = "https://projapan.ru/bikes?mileage_min=0&mileage_max=100&year_min=2009&year_max=2021&rank_min=0&rank_max=5&auction%5B%5D=auc&auction%5B%5D=bds&auction%5B%5D=jba&auction%5B%5D=arai&date=all&volume=50&manufacturer=H&model="
    let client = new HttpClient()
    async {
        let message = new HttpRequestMessage(HttpMethod.Get, request)
        message.Headers.Add ( "Host", "projapan.ru" )
        message.Headers.Add ( HttpRequestHeader.ContentType.ToString(), "application/json; charset=utf-8" )
        message.Headers.Add ( "Accept", "*/*" )
        message.Headers.Add ( "Connection", "keep-alive" )

        let! response = client.SendAsync(message) |> Async.AwaitTask
        response.EnsureSuccessStatusCode () |> ignore
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        let json = JsonSerializer.Deserialize<Root> content
        Seq.iter (fun b -> printfn $"%s{b._id.ToString()}" ) json.bikes
        ()
    } |>  Async.RunSynchronously


    let message = from "F#" // Call the function
    printfn "Hello world %s" message
    0 // return an integer exit code