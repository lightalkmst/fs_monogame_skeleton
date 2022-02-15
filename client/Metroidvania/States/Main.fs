namespace game

open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content

open Globals
open Globals.Values
open Globals.Values.Constants
open Globals.State
open Json
open Logging
open Input
open Parser
open States
open States.Updating
open States.Drawing
open Config

module Main =
  let get_state = function Main state -> state | _ -> failwith "States.Main.get_state"

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
    let state = get_state data.present
    let main = Assets.get_image main
    // draw background
    data.sprite_batch.Draw (main, Rectangle(0, 0, ui_buffer_width, ui_buffer_height), Color.White)

    let draw_str_centered = draw_string_centered Constants.font data

    // draw title
    [Constants.game_name]
    |> List.iteri (fun i x ->
      draw_str_centered 1.0f (new Vector2 (float32 ui_buffer_width / 2.f, 150.f + float32 i * 72.f)) x
    )

    // draw menu items
    // TODO: flashy selection animation
    [
      "Story"
      "Multiplayer"
      "Deckbuilder"
      "Settings"
      //"Library"
    ]
    |> List.iteri (fun i x ->
      draw_str_centered 0.8f (new Vector2 (float32 ui_buffer_width / 2.f, 650.f + float32 i * 60.f)) (if state.main_selection_y = i then "> " + x + " <" else x)
    )

  ()