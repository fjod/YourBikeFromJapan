module Server.Types

open Shared

module DbTypes =
    type DBBikeToSearch =
        { Maker: string
          MinYear: string
          MaxYear: string }
//One or more errors occurred. (A parameterless default constructor or one matching signature (System.String Maker, System.String Model, System.Int16 Year)
//           is required for Server.Types+DbTypes+DbBikeModel materialization)
    type DbBikeModel =
        {
          Maker: System.String
          Model: System.String
          Year: System.Int16
         }

    let ConvertFromTuple(tuple:string*Manufacturer*int) =
        let model,maker,year = tuple
        {Maker= BikeRangeHelper.ManufacturerToString(maker); Model = model; Year = System.Convert.ToInt16 year}