module Client.Register
open Client.MessageTypes
open Elmish
open ClientModel
open Shared
open Client.Cookies
open Client.Login

let workWithRegister (model: Model) (msg: RegisterState) (todosApi: ITodosApi) : Model * Cmd<RegisterState> =
    match msg with
    | Register ->
        let result = Cmd.OfAsync.perform todosApi.register model.InputData RegisterResult
        model,result
    | RegisterResult data ->
        setTokenValue data.Token.Value
        { model with
           LoginState = getVal data.Message
           Token =  data.Token  },   Cmd.none
    | ValidateToken tokenIsFine ->
        match tokenIsFine with
        | true ->
            let model = { model with LoginState = "Logged in" } // State = BikesScreen;
            let command =  RegisterState.GetBikesForUser |> Cmd.ofMsg
            model, command
        | _ -> model, Cmd.none
    | GetBikesForUser ->
        match model.Token with
        | Some t ->
            let result = Cmd.OfAsync.perform todosApi.getBikesForUser t UserBikesResult
            model,result
        | _ ->  model, Cmd.none
    |  UserBikesResult bikes->
        match bikes with
        | Ok b ->  {model with UserRequestedBikes = b}, Cmd.none
        | Error e -> model, Cmd.none
    | TryValidateToken token ->
          let ret = Cmd.OfAsync.perform todosApi.validateToken token RegisterState.ValidateToken
          model,ret
