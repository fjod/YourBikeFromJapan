module Client.BikesScreen.BikeScreenFuncs

open Client
open Client.ClientModel
open Client.MessageTypes
open Elmish
open Shared
open Cookies



let requestBikeModelsIfPossible(model: Model) (todosApi: ITodosApi) :  Cmd<BikeScreenState> =
    let range = BikeRangeFromModel model
    match range, findTokenValue() with
    | Some r, Ok t ->
        Cmd.OfAsync.perform todosApi.getBikesFromRange (t,r)  BikeScreenState.ReturnedModels
    | _ -> Cmd.none


let workBikeScreenUi (model: Model) (msg: BikeScreenState) (todosApi: ITodosApi) : Model * Cmd<BikeScreenState> =

   match msg with
   | SetStartYear v -> {model with StartYear = Some v}, requestBikeModelsIfPossible {model with StartYear = Some v} todosApi
   | SetEndYear v -> {model with EndYear = Some v} , requestBikeModelsIfPossible {model with EndYear = Some v} todosApi
   | SelectedManufacturerName v -> {model with SelectedManufacturer = Some v}, requestBikeModelsIfPossible {model with SelectedManufacturer = Some v} todosApi
   | SelectedModel v -> {model with SelectedModel = Some v}, Cmd.none
   | AddBike r ->
       match findTokenValue() with
       | Ok t ->
            let result = Cmd.OfAsync.perform todosApi.addBike (t,r)  BikeScreenState.BikeAdded
            model, result
       | Error e ->  model, Cmd.none
   | BikeAdded bike->
       match bike with
       | Ok b ->
            let test = Array.append model.UserRequestedBikes [|b|]
            {model with UserRequestedBikes = test}, Cmd.none //user added bike; display it somewhere ?
       | Error _ ->    model,   Cmd.none
   | ReturnedModels models ->
       match models with
       | Ok m ->
            {model with Models = m}, Cmd.none
       | Error _ ->    model,   Cmd.none
