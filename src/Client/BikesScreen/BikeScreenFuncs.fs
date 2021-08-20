module Client.BikesScreen.BikeScreenFuncs

open Client
open Client.ClientModel
open Client.MessageTypes
open Client.MessageTypes
open Elmish
open Shared
open Cookies

let requestBikeModelsIfPossible(model: Model) (todosApi: ITodosApi) :  Cmd<BikeScreenState> =
    let range = BikeRangeFromModel model
    let token = findTokenValue()
    match range, token with
    | Some r, Ok t ->
        Cmd.OfAsync.perform todosApi.getBikesFromRange (t,r)  BikeScreenState.ReturnedModels
    | _ -> Cmd.none


let workBikeScreenUi (model: Model) (msg: BikeScreenState) (todosApi: ITodosApi) : Model * Cmd<BikeScreenState> =
   match msg with
   | SetStartYear v -> {model with StartYear = Some v}, requestBikeModelsIfPossible model todosApi
   | SetEndYear v -> {model with EndYear = Some v} , requestBikeModelsIfPossible model todosApi
   | SelectedManufacturerName v -> {model with SelectedManufacturer = Some v}, requestBikeModelsIfPossible model todosApi
   | SelectedModel v -> {model with SelectedModel = Some v}, Cmd.none
   | AddBike r ->
       let result = Cmd.OfAsync.perform todosApi.addBike ("",r)  BikeScreenState.BikeAdded
       model, result
   | BikeAdded bike->
       model, Cmd.none //user added bike; display it somewhere ?
   | ReturnedModels models ->
       {model with Models = models}, Cmd.none
