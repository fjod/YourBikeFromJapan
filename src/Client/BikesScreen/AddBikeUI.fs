module Client.BikesScreen.AddBikeUI
open Client.ClientModel
open Client.ClientMsg
open Feliz
open Feliz.Bulma
open Fulma
open Shared



let containerAddBike (model: Model) (dispatch: Msg2 -> unit) =
    Bulma.container[
        prop.children[
             Bulma.dropdown[
             for bike in model.UserRequestedBikes ->
                        Bulma.button.button [ prop.text (bike.Model + bike.Year.ToString()) ] //TODO: dispatch command on user click button
                         ]
                      ]

                ]



