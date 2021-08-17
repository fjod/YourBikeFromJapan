module Client.BikesScreen.BikeScreenFuncs

open Client.ClientModel
open Client.MessageTypes
open Client.MessageTypes
open Elmish
open Shared

let workBikeScreenUi (model: Model) (msg: BikeScreenState) : Model * Cmd<BikeScreenState> =
   match msg with
   | SetStartYear v -> {model with StartYear = Some v} , Cmd.none
   | SetEndYear v -> {model with EndYear = Some v} , Cmd.none
   | SelectedManufacturerName v -> {model with SelectedManufacturer = Some v} , Cmd.none
