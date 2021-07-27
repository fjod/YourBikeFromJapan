module Index

open Elmish
open Fable.Remoting.Client
open Shared

type Model = {
    Todos: Todo list
    Input: string
    LoginState : string
    LoginData : LoginInfo
    Token : string
}

type Msg =
    | GotTodos of Todo list
    | SetInput of string
    | AddTodo
    | AddedTodo of Todo
    | Login
    | LogInResult of LoginResult
    | Register
    | RegisterResult of LoginResult
    | SetEmail of string
    | SetPassword of string

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>

let init () : Model * Cmd<Msg> =
    let model = { Todos = []; Input = ""; LoginState = "not logged in"; LoginData = {Email = ""; Password = ""}; Token = "" }

    let cmd =
        Cmd.OfAsync.perform todosApi.getTodos () GotTodos

    model, cmd

let getVal s =
    match s with
    | Some value -> value
    | _ -> "error"

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | GotTodos todos -> { model with Todos = todos }, Cmd.none
    | SetInput value -> { model with Input = value }, Cmd.none
    | SetEmail v ->
        let newData = {model.LoginData with Email = v}
        {model with LoginData = newData},Cmd.none
    | SetPassword v ->
        let newData = {model.LoginData with Password = v}
        {model with LoginData = newData},Cmd.none

    | AddTodo ->
        let todo = Todo.create model.Input

        let cmd =
            Cmd.OfAsync.perform todosApi.addTodo todo AddedTodo

        { model with Input = "" }, cmd
    | AddedTodo todo ->
        { model with
              Todos = model.Todos @ [ todo ] },
        Cmd.none
    | Login  ->
       let result = Cmd.OfFunc.perform todosApi.login model.LoginData LogInResult
       model,result
    | LogInResult data ->
        { model with
           LoginState = getVal data.Message
           Token = data.Token            },
        Cmd.none

    | Register  ->
       let result = Cmd.OfFunc.perform todosApi.login model.LoginData LogInResult
       model,result
    | RegisterResult data ->
        { model with
           LoginState = getVal data.Message
           Token = data.Token            },
        Cmd.none



open Feliz
open Feliz.Bulma

let navBrand =
    Bulma.navbarBrand.div [
        Bulma.navbarItem.a [
            prop.href "https://safe-stack.github.io/"
            navbarItem.isActive
            prop.children [
                Html.img [
                    prop.src "/favicon.png"
                    prop.alt "Logo"
                ]
            ]
        ]
    ]

let containerBox (model: Model) (dispatch: Msg -> unit) =
    Bulma.box [
        Bulma.content [
            Html.ol [
                for todo in model.Todos do
                    Html.li [ prop.text todo.Description ]
            ]
        ]
        Bulma.field.div [
            field.isGrouped
            prop.children [

                Bulma.control.p [
                    control.isExpanded
                    prop.children [
                        Bulma.input.text [
                            prop.value model.Input
                            prop.placeholder "What needs to be done?"
                            prop.onChange (fun x -> SetInput x |> dispatch)
                        ]
                    ]
                ]

                Bulma.control.p [
                    Bulma.button.a [
                        color.isPrimary
                        prop.disabled (Todo.isValid model.Input |> not)
                        prop.onClick (fun _ -> dispatch AddTodo)
                        prop.text "Add"
                    ]
                ]

                Bulma.control.p [
                    control.isExpanded
                    prop.children [
                        Bulma.input.text [
                            prop.value model.Input
                            prop.placeholder "Email"
                            prop.onChange (fun x -> SetEmail x |> dispatch)
                        ]
                    ]
                ]

                Bulma.control.p [
                    control.isExpanded
                    prop.children [
                        Bulma.input.text [
                            prop.value model.Input
                            prop.placeholder "Password"
                            prop.onChange (fun x -> SetPassword x |> dispatch)
                        ]
                    ]
                ]

                Bulma.control.p [
                    Bulma.button.a [
                        color.isPrimary
                        prop.disabled (Todo.isValid model.Input |> not)
                        prop.onClick (fun _ -> dispatch Register)
                        prop.text "Register"
                    ]
                ]

                Bulma.control.p [
                    Bulma.button.a [
                        color.isPrimary
                        prop.disabled (Todo.isValid model.Input |> not)
                        prop.onClick (fun _ -> dispatch Login)
                        prop.text "Login"
                    ]
                ]
            ]
        ]
    ]

let view (model: Model) (dispatch: Msg -> unit) =
    Bulma.hero [
        hero.isFullHeight
        color.isPrimary
        prop.style [
            style.backgroundSize "cover"
            style.backgroundImageUrl "https://unsplash.it/1200/900?random"
            style.backgroundPosition "no-repeat center center fixed"
        ]
        prop.children [
            Bulma.heroHead [
                Bulma.navbar [
                    Bulma.container [ navBrand ]
                ]
            ]
            Bulma.heroBody [
                Bulma.container [
                    Bulma.column [
                        column.is6
                        column.isOffset3
                        prop.children [
                            Bulma.title [
                                text.hasTextCentered
                                prop.text "YourBikeFromJapan"
                            ]
                            containerBox model dispatch
                        ]
                    ]
                ]
            ]
        ]
    ]
