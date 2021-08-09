module Client.LoginPageUI

open Client.MessageTypes
open Elmish
open ClientModel
open Shared
open Client.Cookies
open Client.Login

let workWithLoginUI (model: Model) (msg: ViewUpdateState) : Model * Cmd<ViewUpdateState> =
    match msg with
    | SetEmail v ->
        let newData = {model.InputData with Email = v}
        {model with InputData = newData},Cmd.none
    | SetPassword v ->
        let newData = {model.InputData with Password = v}
        {model with InputData = newData},Cmd.none
