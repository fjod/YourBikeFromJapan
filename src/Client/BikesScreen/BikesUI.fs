module Client.BikesScreen.BikesUI

open Client.ClientModel
open Client.ClientMsg
open Feliz
open Feliz.Bulma
open RequestedBikesUI
open AddBikeUI
open BikesHistoryUI
open AuctDataUI

let containerBikes (model: Model) (dispatch: Msg2 -> unit) =
     Bulma.box[
        Bulma.columns [
            Bulma.column [
                column.is2
                prop.children [
                    containerWithRequestedBikes model dispatch
                    containerAddBike model dispatch
                ]
            ] //1st column
            Bulma.column[
                column.is2
                prop.children [
                    Bulma.button.button [
                        Bulma.color.isSuccess
                        prop.text "Get all bikes"
                    ]
                    containerBikesHistory(model)
                ]
            ]//2rd column
            Bulma.column[
                column.is8
                prop.children [
                   containerAuctData(model)
                   ]
            ]//3rd column
        ]//end columns
     ] //end Bulma.box