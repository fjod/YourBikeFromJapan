module Server.BikeAPI

open System
open System.Net
open System.Net.Http
open System.Text
open System.Text.Json
open System.Text.Json.Serialization
open Shared
open FSharp.Control.Tasks.V2

[<CLIMutable>]
type Id = {
    [<JsonPropertyName("$oid")>]
    Oid:string
}

[<CLIMutable>]
type MetaData = {ranking_status : string}

[<CLIMutable>]
type APIBike = {
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
type Root = {bikes : APIBike seq  }

let client = new HttpClient()
let builder = StringBuilder()
let volumes = [|"rst";"750";"400"|]
let createUriByParams(input:BikeRange)(volume:string) =
    let manufacturerLetter = input.Maker.ToString().[0]
    builder.Clear()
     .Append($"https://projapan.ru/bikes?mileage_min=0&mileage_max=100&year_min={input.StartYear}&year_max={input.EndYear}&rank_min=0&rank_max=5&auction")
     .Append("%5B%5D=auc&auction%5B%5D=bds&auction%5B%5D=jba&auction%5B%5D=arai&date=all")
     .Append($"&volume={volume}&manufacturer={manufacturerLetter}&model=")
     .ToString()

let requestBike2 (uri:string) =
    task{
        let message = new HttpRequestMessage(HttpMethod.Get, uri)
        message.Headers.Add ( "Host", "projapan.ru" )
        message.Headers.Add ( HttpRequestHeader.ContentType.ToString(), "application/json; charset=utf-8" )
        message.Headers.Add ( "Accept", "*/*" )
        message.Headers.Add ( "Connection", "keep-alive" )
        let! response = client.SendAsync(message)
        response.EnsureSuccessStatusCode () |> ignore
        let! content = response.Content.ReadAsStringAsync()
        let json = JsonSerializer.Deserialize<Root> content
        let conv = Seq.map (fun b ->  {Manufacturer = Honda
                                       Model = b.model
                                       Year = 0
                                      }) json.bikes
        return conv
    }

let requestBike (uri:string) : Async<Bike seq> =
    async{
        let message = new HttpRequestMessage(HttpMethod.Get, uri)
        message.Headers.Add ( "Host", "projapan.ru" )
        message.Headers.Add ( HttpRequestHeader.ContentType.ToString(), "application/json; charset=utf-8" )
        message.Headers.Add ( "Accept", "*/*" )
        message.Headers.Add ( "Connection", "keep-alive" )
        let! response = client.SendAsync(message) |> Async.AwaitTask
        response.EnsureSuccessStatusCode () |> ignore
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        let json = JsonSerializer.Deserialize<Root> content
        let conv = Seq.map (fun b ->  {Manufacturer = Honda
                                       Model = b.model
                                       Year = 0
                                      }) json.bikes
        return conv
    }

let createAllRequests (uri:string -> string) =
    volumes |> Array.map (fun v -> requestBike(uri(v)))


let getBikeModelsForRange  (input:BikeRange): string[] =
    let request = createUriByParams input
    createAllRequests request  |> Async.Parallel  |> Async.RunSynchronously |> Seq.collect id |> Seq.map (fun b -> b.Model) |>  Seq.toArray

