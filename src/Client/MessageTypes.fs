module Client.MessageTypes

open Shared

type LoginState =
     | Login
     | LogInResult of LoginResult

type RegisterState =
     | Register
     | RegisterResult of LoginResult
     | ValidateToken of bool

type TodoState =
    | GotTodos of Todo list
    | AddTodo
    | AddedTodo of Todo

type ViewUpdateState =
     | SetEmail of string
     | SetPassword of string
     | SetInput of string