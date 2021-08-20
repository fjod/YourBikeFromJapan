module Client.MessageTypes

open Shared

type LoginState =
     | Login
     | LogInResult of LoginResult

type RegisterState =
     | Register
     | RegisterResult of LoginResult
     | ValidateToken of bool
     | TryValidateToken of string

type ViewUpdateState =
     | SetEmail of string
     | SetPassword of string

type BikeScreenState =
    | SelectedManufacturerName of string
    | SelectedModel of string
    | SetStartYear of string
    | SetEndYear of string
    | ReturnedModels of string[]
    | AddBike of BikeRange
    | BikeAdded of string