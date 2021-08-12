module Client.WelcomeScreen.WelcomeUI

open Client.ClientModel
open Client.ClientMsg
open Feliz
open Feliz.Bulma
open Client.MessageTypes
open Shared
open System

let isEmailAndPasswordValid (data: LoginInfo)=

       let em = String.IsNullOrWhiteSpace data.Email |> not //TODO: proper validation
       let p = String.IsNullOrWhiteSpace data.Password |> not
       em && p


let containerWelcome (model: Model) (dispatch: Msg2 -> unit) =
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