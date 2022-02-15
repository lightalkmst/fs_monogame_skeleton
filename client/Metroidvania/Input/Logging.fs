namespace game

open System
open System.IO
open System.Text

open Globals
open Config.Constants

module Logging =
  //File.WriteAllText (log_file, "")

  let log: string -> unit = 
    //let fs = File.Create log_file
    fun x ->
      Console.WriteLine x
    //  let info = (UTF8Encoding true).GetBytes x
    //  fs.Write (info, 0, info.Length)

  