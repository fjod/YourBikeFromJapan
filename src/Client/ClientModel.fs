module Client.ClientModel

open Shared

type UIState =
    | WelcomeScreen
    | BikesScreen



type Model = {
    UserRequestedBikes : Bike[]
    Input: string
    LoginState : string
    InputData : LoginInfo
    Token : string option
    State : UIState
}
