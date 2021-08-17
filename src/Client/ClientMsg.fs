module Client.ClientMsg
open Client.MessageTypes

type Msg2 =
    | LoginMsg of LoginState
    | RegisterMsg of RegisterState
    | ViewUpdateMsg of ViewUpdateState
    | BikeScreenMsg of BikeScreenState


