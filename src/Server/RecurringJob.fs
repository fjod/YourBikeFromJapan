module Server.RecurringJob
open FSharp.Control.Websockets
open Shared
open System
open BikeAPI
open Database
let GetAuctionData()=

    //sleep for a day

    //делаем запрос вида select min startyear, max endyear, distict maker
    //по этому запросу получаем байки с апи для всех производителей = 12 запросов (набор 1)
    //далее берем select distinct model bikeforuser join bikemodel on ..  чтобы получить только имена байков (набор 2)
    //по каждой модели в наборе 2 нужно выбрать из набора 1 соответствие и посмотреть, нету ли их уже в данных аукционов
    //взять какой-то auct data ключи чтобы не перезаписывать
    //если нету то сохранить

    ()

let wait5AndCall(range:BikeRange) =
    async {
    Console.Write "waiting with range"
    Console.WriteLine range.Maker
    let timer = new Timers.Timer(5000.)
    let event = Async.AwaitEvent timer.Elapsed |> Async.Ignore
    let! r = getDBBikeModelsForRange range
    Console.Write "got info for range"
    Console.WriteLine range.Maker
    return r
}
let prefill() =
 async{
    let (yamahaRange:BikeRange) = {Maker = Yamaha; Model = ""; StartYear = "2000"; EndYear = "2020"}
    let kawaRange = {yamahaRange with Maker = Kawasaki}
    let hondaRange = {yamahaRange with Maker = Honda}
    let sRange = {yamahaRange with Maker = Suzuki}
    let ranges = [|yamahaRange;kawaRange;hondaRange;sRange|]

    for range in ranges do
        let! models = wait5AndCall range
        let! _ = fillBikeModelTable models
        ()

    }