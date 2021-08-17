module Client.BikesScreen.AddBikeUI

open Client.ClientModel
open Client.ClientMsg
open Fable.React.Props
open Feliz
open Feliz.Bulma
open Fulma
open Shared
open Fable.React
open Fable.FontAwesome
open Microsoft.FSharp.Reflection
open Client.MessageTypes

let createManufacturerDropdown (dispatch: Msg2 -> unit) =
    let cases = FSharpType.GetUnionCases typeof<Manufacturer>
    Dropdown.dropdown [ Dropdown.IsHoverable
                         ] [
        Dropdown.trigger [] [
            Button.button [] [
                span [] [ str "Select Maker" ]
                Icon.icon [ Icon.Size IsSmall ] [
                    Fa.i [ Fa.Solid.AngleDown ] []
                ]
            ]
        ]
        Dropdown.menu [  ] [
            Dropdown.content [] [
                for m in cases ->
                 Dropdown.Item.a [  Dropdown.Item.Props[  OnClick (fun _ -> SelectedManufacturerName m.Name |> BikeScreenMsg |> dispatch)  ] ]
                     [
                     str m.Name
                 ]
            ]
        ]
    ]

let createStartYear (model: Model) (dispatch: Msg2 -> unit) =
      Bulma.control.p [
                    control.isExpanded
                    prop.children [
                        Bulma.input.text [
                            prop.value model.StartYear.Value
                            prop.placeholder "Enter Start Year"
                            prop.onChange (fun x -> SetStartYear x |> BikeScreenMsg  |> dispatch)
                        ]
                    ]
                ]

let createEndYear (model: Model) (dispatch: Msg2 -> unit) =
      Bulma.control.p [
                    control.isExpanded
                    prop.children [
                        Bulma.input.text [
                            prop.value model.EndYear.Value
                            prop.placeholder "Enter End Year"
                            prop.onChange (fun x -> SetEndYear x |> BikeScreenMsg  |> dispatch)
                        ]
                    ]
                ]

let containerAddBike (model: Model) (dispatch: Msg2 -> unit) =
    Bulma.container [ prop.children [
        createManufacturerDropdown(dispatch)
        createStartYear model dispatch
        createEndYear model dispatch
    ]
                       ]
