namespace game

open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content

open Globals
open Globals.State
open Json
open Logging
open Input
open Parser
open States.Updating
open States.Drawing
open Config

module Dungeon =
  let get_state = function Dungeon state -> state | _ -> failwith "States.Dungeon.get_state"

  let update: game_updater = fun data ->
    let state = get_state data.present

    // menu view
    match () with
    | _ when Array.contains Confirm data.inputs.actions.[0].Value ->
      maintain_state data
    | _ ->
      //let _, _, y = navigate (data.inputs.dx, -data.inputs.dy) (0, state.main_selection_y) <| List.map (fun x -> ((), 0, x)) [0 .. 3]
      //maintain_state_with_sounds data 
      //<| Main (data.game_time.TotalGameTime.Milliseconds, {
      //  state with      
      //    main_selection_y = y
      //})
      //<| (
      //  if y <> state.main_selection_y
      //  then [{hero = None; sound = "tonk"}]
      //  else []
      //)
      maintain_state data

  let draw: game_drawer = fun data ->
    ()