module Client.BikesScreen.BikeScreenFuncs

open Client.ClientModel
open Client.MessageTypes
open Client.MessageTypes
open Elmish
open Shared

let workBikeScreenUi (model: Model) (msg: BikeScreenState) (todosApi: ITodosApi) : Model * Cmd<BikeScreenState> =
   match msg with
   | SetStartYear v -> {model with StartYear = Some v} , Cmd.none
   | SetEndYear v -> {model with EndYear = Some v} , Cmd.none
   | SelectedManufacturerName v -> {model with SelectedManufacturer = Some v} , Cmd.none
   | AddBike r ->
       let result = Cmd.OfAsync.perform todosApi.addBike ("",r)  BikeScreenState.BikeAdded
       model, result
   | BikeAdded bike->
       model, Cmd.none