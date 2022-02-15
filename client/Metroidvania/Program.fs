open System   
open FSharp.Data

open game

open Globals
open Logging

try
  if Config.env = "local"
  then Console.SetWindowSize (180, 50)
  (new game.Engine.Engine ()).Run()
with
| e -> 
  log e.Message
  if e.InnerException <> null
  then log e.InnerException.Message
  log e.StackTrace
  ignore <| Console.ReadLine ()
  () // TODO: fatal error handling
