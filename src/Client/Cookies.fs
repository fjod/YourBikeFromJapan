module Client.Cookies

open Shared.NullHelper

let findTokenValue ()=
    let token = Browser.WebStorage.localStorage.getItem "BikeToken"
    convert token


let setTokenValue v =
    Browser.WebStorage.localStorage.setItem ("BikeToken", v)