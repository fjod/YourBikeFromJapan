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
open LoginPageFuncs
open Client.WelcomeScreen.WelcomeUI
open Client.BikesScreen.BikeScreenFuncs

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>


let init () : Model * Cmd<Msg2> =
    let currentToken = findTokenValue()
    match currentToken with
    | Ok t ->
        let model = { Input = ""; LoginState = "Checking token"; InputData = {Email = ""; Password = ""}
                      Token = Some t; State = WelcomeScreen; UserRequestedBikes = Array.Empty(); StartYear = None; EndYear = None; SelectedManufacturer = None }
        let q = RegisterState.TryValidateToken t |> Cmd.ofMsg |> Cmd.map RegisterMsg
        model, q
    | Error _ ->
        let model = { Input = ""; LoginState = "Not logged in"; InputData = {Email = ""; Password = ""}
                      Token = None; State = WelcomeScreen;UserRequestedBikes = Array.Empty(); StartYear = None; EndYear = None; SelectedManufacturer = None }
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
     | BikeScreenMsg n ->
         let model, cmd = workBikeScreenUi model n
         model, Cmd.map BikeScreenMsg cmd

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
                            containerWelcome model dispatch
                        ]
                    ]
                ]
            ]
        ]
    ]
