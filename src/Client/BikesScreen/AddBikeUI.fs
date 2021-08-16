module Client.BikesScreen.AddBikeUI
open Client.ClientModel
open Client.ClientMsg
open Feliz.Bulma
open Fulma
open Shared



let containerAddBike (model: Model) (dispatch: Msg2 -> unit) =
    let dr = Bulma.dropdown[

        for man in Manufacturer ->
            dropdownItem[]
    ]
    Bulma.button.button "Click me"