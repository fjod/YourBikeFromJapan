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
            let bike1 = {Manufacturer = Honda; Year = 2005; Model = "Test"}
            let bike2 = {Manufacturer = Yamaha; Year = 2007; Model = "Test2"}
            let bike3 = {Manufacturer = Ducati; Year = 2001; Model = "Test3"}
            let model = { model with LoginState = "Logged in"; State = BikesScreen; UserRequestedBikes = [|bike1;bike2;bike3|] }
            model, Cmd.none //TODO: cmd should be changed to get relevant bikes!
        | _ -> model, Cmd.none
    | TryValidateToken token ->
          let ret = Cmd.OfAsync.perform todosApi.validateToken token RegisterState.ValidateToken
          model,ret
