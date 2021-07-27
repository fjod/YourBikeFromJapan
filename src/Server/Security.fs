module Server.Security

open System
open System.IO
open System.Security.Cryptography
open System.Text
open System.Text.Json
open Shared

type UserInfo =    { Email : string }
let createRandomKey() =
    let generator = System.Security.Cryptography.RandomNumberGenerator.Create()
    let randomKey = Array.init 32 byte
    generator.GetBytes(randomKey)
    randomKey

let private passPhrase =
    let securityTokenFile = FileInfo(Environment.securityTokenFile)
    if not securityTokenFile.Exists then
        let passPhrase = createRandomKey()
        File.WriteAllBytes(securityTokenFile.FullName, passPhrase)
    File.ReadAllBytes(securityTokenFile.FullName)

let private encodeString (payload : string) =
    Jose.JWT.Encode(payload, passPhrase, Jose.JweAlgorithm.A256KW, Jose.JweEncryption.A256CBC_HS512)
let private decodeString (jwt : string) =
    Jose.JWT.Decode(jwt, passPhrase, Jose.JweAlgorithm.A256KW, Jose.JweEncryption.A256CBC_HS512)


let encodeJwt token = JsonSerializer.Serialize token |> encodeString

let decodeJwt<'a> (jwt : string) : 'a =
    decodeString jwt |> JsonSerializer.Deserialize<'a>

// I dont think it validates anything
let validateJwt (jwt : string) : UserInfo option =
    try
        let token = decodeJwt jwt
        Some token
    with _ -> None

let utf8Bytes (input : string) = Encoding.UTF8.GetBytes(input)
let base64 (input : byte []) = Convert.ToBase64String(input)
let sha256 = SHA256.Create()
let sha256Hash (input : byte []) : byte [] = sha256.ComputeHash(input)

let verifyPassword password saltBase64 hashBase64 =
    let salt = Convert.FromBase64String(saltBase64)
    Array.concat [ salt
                   utf8Bytes password ]
    |> sha256Hash
    |> base64
    |> (=) hashBase64

let authorize (f : 'u -> UserInfo -> 't) : SecureRequest<'u> -> SecureResponse<'t> =
    fun request ->
        match validateJwt request.Token with
        | None -> async { return Result.Error AuthError.TokenInvalid }
        | Some user ->
                async {
                    let output = f request.Body user
                    return Result.Ok output
                }