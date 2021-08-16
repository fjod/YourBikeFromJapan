module Client.BikesScreen.RequestedBikesUI
open Client.ClientModel
open Client.ClientMsg
open Feliz
open Feliz.Bulma
open Shared



let containerWithRequestedBikes (model: Model) (dispatch: Msg2 -> unit) =
    Bulma.container[
        prop.children[
            for bike in model.UserRequestedBikes ->
                Bulma.button.button [ prop.text (bike.Model + bike.Year.ToString()) ] //TODO: dispatch command on user click button
        ]
    ]
    //Bulma.button.button "Click me"
