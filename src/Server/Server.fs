module Server.Server


open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Saturn

open Shared
open Server.WelcomeScreen

//https://github.com/demystifyfp/FsConfig  look for env vars

let todosApi =
    {
      login = fun data -> async { return login(data) }
      register = fun data -> async { return register(data) }
      validateToken = fun data -> async { return validate(data) }
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
