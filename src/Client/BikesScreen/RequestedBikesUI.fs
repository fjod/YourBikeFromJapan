module Client.BikesScreen.RequestedBikesUI
open Client.ClientModel
open Client.ClientMsg
open Feliz
open Feliz.Bulma
open Shared



let containerWithRequestedBikes (model: Model) (dispatch: Msg2 -> unit) =
    Bulma.container[
        prop.children[
            Bulma.panel[
                        for bike in model.UserRequestedBikes ->
                            Bulma.panelBlock.div[
                                Bulma.button.button [ prop.text ($"{bike.Model} {bike.StartYear} - {bike.EndYear}") ] //TODO: dispatch command on user click button
                            ]
                ]
        ]
    ]

