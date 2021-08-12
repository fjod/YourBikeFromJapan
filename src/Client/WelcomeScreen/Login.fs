module Client.Login


open Client.MessageTypes
open Elmish
open ClientModel
open Shared

let getVal s =
    match s with
    | Some value -> value
    | _ -> "error"

let workWithLogin (model: Model) (msg: LoginState) (todosApi: ITodosApi) : Model * Cmd<LoginState> =
    match msg with
    | LoginState.Login ->
        let result =
            Cmd.OfAsync.perform todosApi.login model.InputData LoginState.LogInResult

        model, result
    | LoginState.LogInResult data ->
        { model with
              LoginState = getVal data.Message
              Token = data.Token },
        Cmd.none


