module Client.BikesScreen.BikeScreenFuncs

open Client.ClientModel
open Client.MessageTypes
open Client.MessageTypes
open Elmish
open Shared

let workBikeScreenUi (model: Model) (msg: BikeScreenState) : Model * Cmd<BikeScreenState> =
   model, Cmd.none
