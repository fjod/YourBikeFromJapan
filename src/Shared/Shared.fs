namespace Shared

open System

type Manufacturer =
    | Honda
    | Suzuki
    | Kawasaki
    | Yamaha


type Bike = {
    Manufacturer : Manufacturer
    Model : string
    Year : int
    Key:string
    Mileage : int
    Image : string
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
        | Some _ -> None
        | None -> None

    let ManufacturerFromLetter (letter : string ) : Manufacturer option =
         match letter with
            |  "H" -> Some Honda
            |  "S" -> Some Suzuki
            |  "K" -> Some Kawasaki
            |  "Y" -> Some Yamaha
            |  _ -> None
    let ManufacturerToString (m :Manufacturer) =
        match m with
        | Honda -> "Honda"
        | Kawasaki -> "Kawasaki"
        | Yamaha -> "Yamaha"
        | Suzuki -> "Suzuki"

type LoginInfo = {Email:string; Password:string}

type LoginResult ={
    Message:string option
    Token:string option
}


module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName



type SecureRequest<'t> = {
    Token : string
    Body : 't
}

type AuthError =
    | TokenInvalid
    | UserUnauthorized
    | NoUserForEmail

type SecureResponse<'t> = Async<Result<'t, AuthError>>

type DbUser = {
    id :int
    email : string
    password : string
    salt : string
}

type ITodosApi =
    { login: LoginInfo -> Async<LoginResult>
      register: LoginInfo -> Async<LoginResult>
      validateToken: string -> Async<bool>
      addBike: string*BikeRange ->Async<Result<BikeRange,AuthError>> //return range of bikes and save it to model
      getBikesFromRange:string*BikeRange->Async<Result<string[],AuthError>>
      getBikesForUser:string->Async<Result<BikeRange[],AuthError>>
    }

type AuctionData = {
    id : int
    Manufacturer : string
    Mileage : int
    Img : string
    Year : int
    BikeKey : string
}

