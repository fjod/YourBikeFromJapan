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
            let bike1 = {Maker = Honda; StartYear = "2005"; Model = "Test"; EndYear = "2010"}
            let bike2 = {Maker = Honda; StartYear = "2001"; Model = "Test1"; EndYear = "2011"}
            let bike3 = {Maker = Honda; StartYear = "2002"; Model = "Test2"; EndYear = "2012"}
            let model = { model with LoginState = "Logged in"; State = BikesScreen; UserRequestedBikes = [|bike1;bike2;bike3|] }
            model, Cmd.none //TODO: cmd should be changed to get relevant bikes!
        | _ -> model, Cmd.none
    | TryValidateToken token ->
          let ret = Cmd.OfAsync.perform todosApi.validateToken token RegisterState.ValidateToken
          model,ret
