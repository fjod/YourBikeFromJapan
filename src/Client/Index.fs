module Index


open Elmish
open Fable.Remoting.Client
open Shared
open Client.Cookies
open System



type Model = {
    Todos: Todo list
    Input: string
    LoginState : string
    InputData : LoginInfo
    Token : string option
}


let isEmailAndPasswordValid (data: LoginInfo)=

       let em = String.IsNullOrWhiteSpace data.Email |> not //TODO: proper validation
       let p = String.IsNullOrWhiteSpace data.Password |> not
       em && p

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>

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
    | ValidateToken of bool

type Msg2 =
    | LoginMsg of Client.MessageTypes.LoginState
    | RegisterMsg of Client.MessageTypes.RegisterState
    | TodoState of Client.MessageTypes.TodoState
    | ViewUpdateMsg of Client.MessageTypes.ViewUpdateState

let init () : Model * Cmd<Msg2> =
    let currentToken = findTokenValue()

    match currentToken with
    | Ok t ->
        let ret = Cmd.OfAsync.perform todosApi.validateToken t ValidateToken
        let model = { Todos = []; Input = ""; LoginState = "Checking token"; InputData = {Email = ""; Password = ""}; Token = Some t }
        model, ret //TODO: cmd should be changed to get relevant bikes
    | Error _ ->
        let model = { Todos = []; Input = ""; LoginState = "Not logged in"; InputData = {Email = ""; Password = ""}; Token = None }
        model, Cmd.none


let workWIthLogin model msg =
     model , Cmd.none

let getVal s =
    match s with
    | Some value -> value
    | _ -> "error"

let update2 (msg: Msg2) (model: Model) : Model * Cmd<Msg2> =
     match msg with
     | LoginMsg n -> workWIthLogin model n
     | RegisterMsg n -> workWIthLogin model n
     | TodoState n -> workWIthLogin model n
     | ViewUpdateMsg n ->  workWIthLogin model n

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | GotTodos todos -> { model with Todos = todos }, Cmd.none
    | SetInput value -> { model with Input = value }, Cmd.none
    | SetEmail v ->
        let newData = {model.InputData with Email = v}
        {model with InputData = newData},Cmd.none
    | SetPassword v ->
        let newData = {model.InputData with Password = v}
        {model with InputData = newData},Cmd.none

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
       let result = Cmd.OfAsync.perform todosApi.login model.InputData LogInResult
       model,result
    | LogInResult data ->
        { model with
           LoginState = getVal data.Message
           Token = data.Token            },
        Cmd.none

    | Register  ->
       let result = Cmd.OfAsync.perform todosApi.register model.InputData RegisterResult
       model,result
    | RegisterResult data ->
        setTokenValue data.Token.Value
        { model with
           LoginState = getVal data.Message
           Token =  data.Token            },
        Cmd.none
    | ValidateToken tokenIsFine ->
        match tokenIsFine with
        | true ->
            let model = { model with LoginState = "Logged in" }
            model, Cmd.none //cmd should be changed to get relevant bikes
        | _ -> model, Cmd.none


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
            field.isGroupedCentered
            prop.children [

                Bulma.label [
                        prop.text ("You are: " + model.LoginState)
                    ]
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
                            prop.value model.InputData.Email
                            prop.placeholder "Email"
                            prop.onChange (fun x -> SetEmail x |> dispatch)
                        ]
                    ]
                ]

                Bulma.control.p [
                    control.isExpanded
                    prop.children [
                        Bulma.input.text [
                            prop.value model.InputData.Password
                            prop.placeholder "Password"
                            prop.onChange (fun x -> SetPassword x |> dispatch)
                        ]
                    ]
                ]

                Bulma.control.p [
                    Bulma.button.a [
                        color.isPrimary
                        prop.disabled (isEmailAndPasswordValid model.InputData |> not)
                        prop.onClick (fun _ -> dispatch Register)
                        prop.text "Register"
                    ]
                ]

                Bulma.control.p [
                    Bulma.button.a [
                        color.isPrimary
                        prop.disabled (isEmailAndPasswordValid model.InputData |> not)
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
