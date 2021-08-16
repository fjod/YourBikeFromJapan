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