module Client.ClientModel

open Shared

type UIState =
    | WelcomeScreen
    | BikesScreen

type Model = {
    Input: string
    LoginState : string
    InputData : LoginInfo
    Token : string option
    State : UIState
}
