module Server.BikeAPI

open System
open System.Net
open System.Net.Http
open System.Text
open System.Text.Json
open System.Text.Json.Serialization
open Server.Types
open Shared
open FSharp.Control.Tasks.V2
open DbTypes
open Database

let options = JsonSerializerOptions()
options.Converters.Add(JsonFSharpConverter())

[<CLIMutable>]
type Id = {
    [<JsonPropertyName("$oid")>]
    Oid:string option
}

[<CLIMutable>]
type MetaData = {ranking_status : string option}

[<CLIMutable>]
type APIBike = {
       metadata : MetaData option;
      _id : Id option;
         auction : string option;
          bid_number :int option;
          category : Object option;
          condition_engine :string option;
          condition_exterior : string option;
          condition_frame : string option;
          condition_front : string option;
          condition_parts : string option;
          condition_rear :string option;
          date : string option;
          engine_volume :int option;
          final_bid :int option;
          final_bid_rub : int option;
          frame_number :string option;
          key :string option;
          lane :Object option;
          manufacturer :string option;
          mileage :int option;
          model :string option;
          model_alise :string option;
          preview_image :string option;
          rank: string option;
          result :string option;
          starting_bid : int option;
          starting_bid_rub : int option;
          state : Object option;
          year :int option;
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

let mapBike (api:APIBike) :  Bike option =
    match api.manufacturer, api.model, api.year, api.key, api.mileage, api.preview_image with
    | Some man, Some model, Some y, Some k, Some mil, Some im ->
                          let manufacturer = BikeRangeHelper.ManufacturerFromLetter man
                          match manufacturer with
                          | Some m ->
                              Some {  Manufacturer = m
                                      Model = model
                                      Year = y
                                      Key = k
                                      Mileage = mil
                                      Image = $"https://projapan.ru{im}"
                                   }
                          | None -> None
    | _ -> None

let requestBike (uri:string)  : Async<Bike option seq> =
    async{
        let message = new HttpRequestMessage(HttpMethod.Get, uri)
        message.Headers.Add ( "Host", "projapan.ru" )
        message.Headers.Add ( HttpRequestHeader.ContentType.ToString(), "application/json; charset=utf-8" )
        message.Headers.Add ( "Accept", "*/*" )
        message.Headers.Add ( "Connection", "keep-alive" )
        let! response = client.SendAsync(message) |> Async.AwaitTask
        response.EnsureSuccessStatusCode () |> ignore
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        let json = JsonSerializer.Deserialize<Root> (content,options)
        let conv = Seq.map mapBike json.bikes

        Console.WriteLine (uri + " returned some info")
        return conv
    }

let createAllRequests (uri:string -> string) =
    volumes |> Array.map (fun v -> requestBike(uri(v)))


let getBikeModelsForRange  (input:BikeRange) =

    async{
        let! bikesFromDb = getModelsForRange input
        return bikesFromDb |> Seq.toArray
    }



let getDBBikeModelsForRange  (input:BikeRange) =

    async {
         let request = createUriByParams input
         let! info = createAllRequests request  |> Async.Parallel
         let result =  info   |> Seq.collect id |> Seq.choose id
                               |> Seq.distinctBy (fun b-> b.Model.Trim())
                               |> Seq.map (fun b -> ConvertFromTuple(b.Model,b.Manufacturer,b.Year))
         return result
    }



