namespace game

open System                           
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content

open Globals
open Globals.Values
open Globals.Values.Constants
open Globals.State
open Input
open States.Updating
open States.Drawing

module Opening =
  let get_state = function Opening (start) -> start | _ -> failwith "States.Opening.get_state"

  let update: game_updater = fun data ->
    {
      dud_update with
        states = 
          if Array.contains Confirm data.inputs.actions.[0].Value
          then
            Main {
              start = data.game_time.TotalGameTime.Milliseconds
              main_selection_y = 0
            }
            :: data.states
          else data.states
    }

  let draw: game_drawer = fun data ->
    //let opening = Assets.get_image opening
    //data.sprite_batch.Draw (opening, Rectangle(0, 0, ui_buffer_width, ui_buffer_height), Color.White)
    ()