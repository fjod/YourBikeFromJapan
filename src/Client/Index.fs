module Index


open Client
open Client.MessageTypes
open Elmish
open Fable.Remoting.Client
open Shared
open Client.Cookies
open System
open ClientModel
open ClientMsg
open Login
open Register
open LoginPageUI

let isEmailAndPasswordValid (data: LoginInfo)=

       let em = String.IsNullOrWhiteSpace data.Email |> not //TODO: proper validation
       let p = String.IsNullOrWhiteSpace data.Password |> not
       em && p

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>


let init () : Model * Cmd<Msg2> =
    let currentToken = findTokenValue()
    match currentToken with
    | Ok t ->
        let model = { Input = ""; LoginState = "Checking token"; InputData = {Email = ""; Password = ""}; Token = Some t }
        let q = RegisterState.TryValidateToken t
        let cmd = Cmd.ofMsg q
        let cmd2 = Cmd.map RegisterMsg cmd
        model, cmd2 //TODO: cmd should be changed to get relevant bikes
    | Error _ ->
        let model = { Input = ""; LoginState = "Not logged in"; InputData = {Email = ""; Password = ""}; Token = None }
        model, Cmd.none



let update2 (msg: Msg2) (model: Model) : Model * Cmd<Msg2> =
     match msg with
     | LoginMsg n ->
         let loginModel, loginCmd = workWithLogin model n todosApi
         loginModel , Cmd.map LoginMsg loginCmd
     | RegisterMsg n ->
           let loginModel, loginCmd = workWithRegister model n todosApi
           loginModel , Cmd.map RegisterMsg loginCmd
     | ViewUpdateMsg n ->
           let loginModel, loginCmd = workWithLoginUI model n
           loginModel , Cmd.map ViewUpdateMsg loginCmd


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

let containerBox (model: Model) (dispatch: Msg2 -> unit) =
    Bulma.box [
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
                            prop.value model.InputData.Email
                            prop.placeholder "Email"
                            prop.onChange (fun x -> SetEmail x |> ViewUpdateMsg |> dispatch)
                        ]
                    ]
                ]

                Bulma.control.p [
                    control.isExpanded
                    prop.children [
                        Bulma.input.text [
                            prop.value model.InputData.Password
                            prop.placeholder "Password"
                            prop.onChange (fun x -> SetPassword x |> ViewUpdateMsg  |> dispatch)
                        ]
                    ]
                ]

                Bulma.control.p [
                    Bulma.button.a [
                        color.isPrimary
                        prop.disabled (isEmailAndPasswordValid model.InputData |> not)
                        prop.onClick (fun _ ->  Register |> RegisterMsg |> dispatch)
                        prop.text "Register"
                    ]
                ]

                Bulma.control.p [
                    Bulma.button.a [
                        color.isPrimary
                        prop.disabled (isEmailAndPasswordValid model.InputData |> not)
                        prop.onClick (fun _ ->  Login |> LoginMsg |> dispatch)
                        prop.text "Login"
                    ]
                ]
            ]
        ]
    ]

let view (model: Model) (dispatch: Msg2 -> unit) =
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
