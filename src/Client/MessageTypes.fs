module Client.MessageTypes

open Shared

type LoginState =
     | Login
     | LogInResult of LoginResult

type RegisterState =
     | Register
     | RegisterResult of LoginResult
     | ValidateToken of bool

type ViewUpdateState =
     | SetEmail of string
     | SetPassword of string
     | SetInput of string