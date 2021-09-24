module Client.BikesScreen.BikesHistoryUI

open System
open Client.ClientModel
open Feliz
open Feliz.Bulma
open Client.MessageTypes
open Client.ClientMsg
//список кнопок отсортированных по дате
// (дата) кол-во записей
let groupedDates (model : Model)=
    let input = model.AuctData
    input |> Array.groupBy (fun d -> d.ScrapedAt)

let getBetterStringForDate(d:DateTime) =
     d.Date.ToString("dd/MM/yyyy")


let containerBikesHistory (model: Model) (dispatch: Msg2 -> unit) =
    let dates = groupedDates model
    Bulma.control.p [
                    control.isExpanded
                    prop.children [
                         for date in dates do
                            Bulma.button.a [
                                            color.isPrimary
                                            prop.onClick (fun _ -> SelectedAuctDate (fst date) |> BikeScreenMsg |> dispatch)
                                            prop.text $"{ getBetterStringForDate((fst date))} , {(snd date).Length} bikes"
                                        ]

                    ]
                ]
