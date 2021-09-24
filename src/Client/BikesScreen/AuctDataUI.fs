module Client.BikesScreen.AuctDataUI

open Client.ClientModel
open Feliz
open Feliz.Bulma

let getAuctInfo(model:Model) =
    match model.SelectedDate with
      |Some date -> model.AuctData |> Array.where (fun d -> d.ScrapedAt = date)
      | _ -> [||]


//взять из модели список объяв по дате, которая равна выбранной дате
let containerAuctData (model: Model) =
    let data = getAuctInfo model
    Bulma.container[
        prop.children[
            for bike in data ->
                           Bulma.card [
                                Bulma.cardImage [
                                        Bulma.image [
                                            Bulma.image.is4by3
                                            prop.children [
                                                Html.img [
                                                    prop.src bike.Img
                                                ]
                                            ]
                                        ]
                                    ]//end cardImage
                                Bulma.cardContent[
                                    Bulma.media [

                                        Bulma.mediaRight[
                                                   Bulma.content $"{bike.Manufacturer} {bike.Model} {bike.Year}"
                                                   Bulma.content $"Mileage {bike.Mileage}"
                                            ]
                                    ]

                                ]//end cardContent

                                Bulma.cardFooter [
                                    Bulma.cardFooterItem.a [
                                        prop.text "Go to on auction site"
                                        prop.href $"https://projapan.ru/bike/{bike.BikeKey}"
                                    ]
                                ]//end cardFooter
                ]//end card
        ]//end prop children
    ]//end container