﻿module Client.BikesScreen.AddBikeUI

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


let getCurrentManufacturer (model : Model) =
    match model.SelectedManufacturer with
    | Some m -> m
    | None -> "Select Maker"
let getCurrentSelectedModel (model : Model) =
    match model.SelectedModel with
    | Some m -> m
    | None -> "Select Model"

let createManufacturerDropdown (model : Model) (dispatch: Msg2 -> unit) =
    let cases = FSharpType.GetUnionCases typeof<Manufacturer>
    Dropdown.dropdown [ Dropdown.IsHoverable
                         ] [
        Dropdown.trigger [] [
            Button.button [] [
                span [] [ str (getCurrentManufacturer model) ]
                Icon.icon [ Icon.Size IsSmall ] [
                    Fa.i [ Fa.Solid.AngleDown ] []
                ]
            ]
        ]
        Dropdown.menu [  ] [
            Dropdown.content [] [
                for m in cases ->
                 Dropdown.Item.a [  Dropdown.Item.Props[  OnClick (fun _ -> SelectedManufacturerName m.Name |> BikeScreenMsg |> dispatch)  ] ] //SelectedManufacturerName no intellisense if remove open dir
                     [
                     str m.Name
                 ]
            ]
        ]
    ]

let createModelsDropdown (model:Model) (dispatch: Msg2 -> unit) =
    Dropdown.dropdown [ Dropdown.IsHoverable
                         ] [
        Dropdown.trigger [] [
            Button.button [] [
                span [] [ str (getCurrentSelectedModel model) ]
                Icon.icon [ Icon.Size IsSmall ] [
                    Fa.i [ Fa.Solid.AngleDown ] []
                ]
            ]
        ]
        Dropdown.menu [  ] [
            Dropdown.content [] [
                for m in model.Models ->
                 Dropdown.Item.a [  Dropdown.Item.Props[  OnClick (fun _ -> SelectedModel m |> BikeScreenMsg |> dispatch)  ] ] //SelectedManufacturerName no intellisense if remove open dir
                     [
                     str m
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

let GetMaker (model:Model) =
    let maker = BikeRangeHelper.BikeRangeFromString model.SelectedManufacturer
    match maker with
    |None -> None
    |Some m ->
          Some {Maker = m;Model = ""; StartYear = ""; EndYear = ""}
let GetStartYear (model:Model) (range: BikeRange option) =
    match (model.StartYear, range) with
    |Some m, Some r ->
          Some {r with StartYear = m}
    | _ -> None
let GetEndYear (model:Model) (range: BikeRange option) =
    match (model.EndYear, range) with
    |Some m, Some r ->
          Some {r with EndYear = m}
    | _ -> None
let GetSelectedBikeModel (model:Model) (range: BikeRange option) =
    match (model.SelectedModel, range) with
    |Some m, Some r ->
          Some {r with Model = m}
    | _ -> None

let GetBikeRange (model:Model) =
   GetMaker model |> GetStartYear model |> GetEndYear model |> GetSelectedBikeModel model

let AddBikeEvent (model: Model) (dispatch: Msg2 -> unit)=
    match GetBikeRange(model) with
    |Some range -> AddBike range |> BikeScreenMsg |> dispatch
    |None -> ()

let containerAddBike (model: Model) (dispatch: Msg2 -> unit) =
    Bulma.container [ prop.children [
        Bulma.panel[
            Bulma.panelHeading [ prop.text "Добавить" ]
            Bulma.panelBlock.div [ createManufacturerDropdown  model dispatch]
            Bulma.panelBlock.div [ createStartYear model dispatch]
            Bulma.panelBlock.div [createEndYear model dispatch]
            Bulma.panelBlock.div [createModelsDropdown model dispatch]
        ]
        Bulma.control.p [
                    Bulma.button.a [
                        color.isPrimary
                        prop.onClick (fun _ -> AddBikeEvent model dispatch)
                        prop.text "Add Bike To Search List"
                    ]
                ]
    ]
                       ]
