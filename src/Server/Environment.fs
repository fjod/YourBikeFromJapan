module Server.Environment

open System.IO
open System
open Microsoft.Extensions.Configuration


let (</>) x y = Path.Combine(x, y)

let BikeProjectFolderName = "bikes"

let dataFolder =
    let appDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData)
    let folder = appDataFolder </> BikeProjectFolderName
    let directoryInfo = DirectoryInfo(folder)
    if not directoryInfo.Exists then Directory.CreateDirectory folder |> ignore
    printfn "Using data folder: %s" folder
    folder


let securityTokenFile = dataFolder </> BikeProjectFolderName

let rec findRoot dir =
    let paketDeps = dir </> "paket.dependencies"
    if File.Exists paketDeps then dir
    else
        let parent = Directory.GetParent(dir)
        if isNull parent then failwith "Couldn't find root directory"
        findRoot parent.FullName

let solutionRoot =
    let cwd = System.Reflection.Assembly.GetEntryAssembly().Location
    let root = findRoot cwd
    root



type Config = {
    connectionString: string
    env : string option
}

let getSettings =
            let ofString = function
                | ""
                | null -> None
                | x -> Some x
            Environment.CurrentDirectory <- (System.Reflection.Assembly.GetExecutingAssembly()).Location
                                                   |> Path.GetDirectoryName
            let configurationRoot =
                ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appSettings.json").AddEnvironmentVariables()
                    .Build()
            let currentEnv = configurationRoot.GetValue("ASPNETCORE_ENVIRONMENT")
            let appConfig = new FsConfig.AppConfig(configurationRoot)
            let result =  appConfig.Get<string>("connectionString")
            { connectionString = result.OkValue; env = ofString currentEnv}