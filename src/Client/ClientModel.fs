module Client.ClientModel

open Shared

type Model = {
    Input: string
    LoginState : string
    InputData : LoginInfo
    Token : string option
}
