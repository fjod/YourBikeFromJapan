module Shared.NullHelper
open System
open Shared
let (|NotNull|_|) input =
           match Object.ReferenceEquals(input,null) with
           | true -> None
           | _ -> Some input


let convert input =
           match input with
           | NotNull o -> Ok o
           | _ -> Error "not found"

let test ()=
    let z = { id =1;    email ="";    password = "";    salt = ""}
    convert z
