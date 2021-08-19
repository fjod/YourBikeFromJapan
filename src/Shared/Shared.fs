namespace Shared

open System

type Manufacturer =
    | Honda
    | Suzuki
    | Kawasaki
    | Yamaha
    | BMW
    | Ducati


type Bike = {
    Manufacturer : Manufacturer
    Model : string
    Year : int
}

type BikeRange= {
    Maker : Manufacturer
    Model : string
    StartYear : string
    EndYear : string
}

module BikeRangeHelper =
    let BikeRangeFromString (word:string option) : Manufacturer option =
        match word with
        | Some "Honda" -> Some Honda
        | Some "Suzuki" -> Some Suzuki
        | Some "Kawasaki" -> Some Kawasaki
        | Some "Yamaha" -> Some Yamaha
        | Some "BMW" -> Some BMW
        | Some "Ducati" -> Some Ducati
        | Some _ -> None
        | None -> None

type LoginInfo = {Email:string; Password:string}

type LoginResult ={
    Message:string option
    Token:string option
}


module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type ITodosApi =
    { login: LoginInfo -> Async<LoginResult>
      register: LoginInfo -> Async<LoginResult>
      validateToken: string -> Async<bool>
      addBike: string*BikeRange ->Async<string> //return range of bikes and save it to model
      getBikesFromRange:string*BikeRange->Async<string[]>
    }

type SecureRequest<'t> = {
    Token : string
    Body : 't
}

type AuthError =
    | TokenInvalid
    | UserUnauthorized
type SecureResponse<'t> = Async<Result<'t, AuthError>>

type DbUser = {
    id :int
    email : string
    password : string
    salt : string
}

type AppUser = {
    email:string
}