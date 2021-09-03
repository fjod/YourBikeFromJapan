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
open Client.BikesScreen.BikesUI

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>


let init () : Model * Cmd<Msg2> =
    let currentToken = findTokenValue()
    match currentToken with
    | Ok t ->
        let model = { Input = ""; LoginState = "Checking token"; InputData = {Email = ""; Password = ""}
                      Token = Some t; State = WelcomeScreen; UserRequestedBikes = [||]
                      StartYear = Some "1990"; EndYear = Some "2020"; SelectedManufacturer = None; Models = [| |]; SelectedModel = None }
        let q = RegisterState.TryValidateToken t |> Cmd.ofMsg |> Cmd.map RegisterMsg
        model, q
    | Error _ ->
        let model = { Input = ""; LoginState = "Not logged in"; InputData = {Email = ""; Password = ""}
                      Token = None; State = WelcomeScreen;UserRequestedBikes = [||]
                      StartYear = Some "1990"; EndYear = Some "2020"; SelectedManufacturer = None; Models = [| |]; SelectedModel = None }
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
         let model, cmd = workBikeScreenUi model n todosApi
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

let getCurrentView (model: Model) (dispatch: Msg2 -> unit) =
    match model.State with
    | WelcomeScreen ->  containerWelcome model dispatch
    | BikesScreen -> containerBikes model dispatch

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
//                    Bulma.column [
//                        column.is12 //TODO: use proper hero column styles for login/bikes screens
//                        column.isOffset1
//                        prop.children [
//                            Bulma.title [
//                                text.hasTextCentered
//                                prop.text "YourBikeFromJapan"
//                            ]
//                            getCurrentView model dispatch
//                        ]
//                    ]
                    getCurrentView model dispatch
                ]
            ]
        ]
    ]
