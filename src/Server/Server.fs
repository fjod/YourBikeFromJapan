module Server.Server


open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Saturn

open Shared
open Server.Types



type Storage() =
    let todos = ResizeArray<_>()

    member __.GetTodos() = List.ofSeq todos

    member __.AddTodo(todo: Todo) =
        if Todo.isValid todo.Description then
            todos.Add todo
            Ok()
        else
            Error "Invalid todo"

let storage = Storage()

storage.AddTodo(Todo.create "Create new SAFE project")
|> ignore

storage.AddTodo(Todo.create "Write your app")
|> ignore

storage.AddTodo(Todo.create "Ship it !!!")
|> ignore

//https://github.com/demystifyfp/FsConfig  look for env vars


let todosApi =
    { getTodos = fun () -> async { return storage.GetTodos() }
      addTodo =
          fun todo ->
              async {
                  match storage.AddTodo todo with
                  | Ok () -> return todo
                  | Error e -> return failwith e
              }
      login = login
      register = register
    }

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue todosApi
    |> Remoting.buildHttpHandler


let app =
    application {

        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }


run app
