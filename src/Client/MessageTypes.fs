module Client.MessageTypes

open System
open Shared

type LoginState =
     | Login
     | LogInResult of LoginResult

type RegisterState =
     | Register
     | RegisterResult of LoginResult
     | ValidateToken of bool
     | TryValidateToken of string
     | GetBikesForUser
     | UserBikesResult of Result<BikeRange[],AuthError>

type ViewUpdateState =
     | SetEmail of string
     | SetPassword of string

type BikeScreenState =
    | SelectedManufacturerName of string
    | SelectedAuctDate of DateTime
    | SelectedModel of string
    | SetStartYear of string
    | SetEndYear of string
    | ReturnedModels of Result<string[],AuthError>
    | AddBike of BikeRange
    | BikeAdded of Result<BikeRange,AuthError>
    | GetAuctData of BikeRange
    | AuctData of Result<AuctionData[],AuthError>