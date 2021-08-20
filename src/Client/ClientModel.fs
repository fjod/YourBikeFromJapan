module Client.ClientModel

open Shared

type UIState =
    | WelcomeScreen
    | BikesScreen



type Model = {
    UserRequestedBikes : BikeRange[]
    Input: string
    LoginState : string
    InputData : LoginInfo
    Token : string option
    State : UIState

    StartYear: string option
    EndYear : string option
    SelectedManufacturer : string option
    Models: string[]
    SelectedModel: string option
}

let BikeRangeFromModel (m:Model) : BikeRange option =
    match m.SelectedManufacturer, m.StartYear, m.EndYear with
    | v, Some s, Some e ->
          let maker = BikeRangeHelper.BikeRangeFromString v
          match maker with
          | Some m ->
             Some {StartYear = s; EndYear = e; Model = ""; Maker = m}
          | None -> None
    | _ -> None