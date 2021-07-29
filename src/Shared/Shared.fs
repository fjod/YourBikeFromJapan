namespace Shared

open System

type Todo = { Id: Guid; Description: string }

type LoginInfo = {Email:string; Password:string}

type LoginResult ={
    Message:string option
    Token:string option
}

module Todo =
    let isValid (description: string) =
        String.IsNullOrWhiteSpace description |> not

    let create (description: string) =
        { Id = Guid.NewGuid()
          Description = description }


module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type ITodosApi =
    { getTodos: unit -> Async<Todo list>
      addTodo: Todo -> Async<Todo>
      login: LoginInfo -> Async<LoginResult>
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