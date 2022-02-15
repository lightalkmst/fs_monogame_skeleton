namespace game

open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content

open Globals
open Globals.Values
open Globals.Values.Constants
open Globals.State
open Config
open Input
open States.Updating
open States.Drawing

module Settings =
  let get_state = function Settings state -> state | _ -> failwith "States.Settings.get_state"

  let allowed_num_chars = [|
    "0"
    "1"
    "2"
    "3"
    "4"
    "5"
    "6"
    "7"
    "8"
    "9"
  |]
  
  //let width_opt = "Screen Width", Constants.buffer_width_f32 * 0.3f, Constants.buffer_height_f32 * 0.2f
  //let height_opt = "Screen Height", Constants.buffer_width_f32 * 0.3f, Constants.buffer_height_f32 * 0.28f
  //let up_opt = "Up", Constants.buffer_width_f32 * 0.3f, Constants.buffer_height_f32 * 0.36f
  //let down_opt = "Down", Constants.buffer_width_f32 * 0.3f, Constants.buffer_height_f32 * 0.44f
  //let left_opt = "Left", Constants.buffer_width_f32 * 0.3f, Constants.buffer_height_f32 * 0.52f
  //let right_opt = "Right", Constants.buffer_width_f32 * 0.3f, Constants.buffer_height_f32 * 0.6f
  //let autologin_opt = "Autologin", Constants.buffer_width_f32 * 0.3f, Constants.buffer_height_f32 * 0.68f
  //let save_opt = "Save", Constants.buffer_width_f32 * 0.3f, Constants.buffer_height_f32 * 0.76f
  //let cancel_opt = "Cancel", Constants.buffer_width_f32 * 0.3f, Constants.buffer_height_f32 * 0.84f
  //let setting_opts = [width_opt; height_opt; up_opt; down_opt; left_opt; right_opt; autologin_opt; save_opt; cancel_opt]

  let update: game_updater = fun data ->
    maintain_state data
    //let (start, state) = get_state data.present
    //let (left_click, right_click) = data.inputs.mouse_clicks

    //if state.focused
    //then 
    //  let get_key () =
    //    if data.inputs.kbd_clicks.Length > 0
    //    then 
    //      let x, _, _, _ = data.inputs.kbd_clicks.[0]
    //      Some x
    //    else None
    //  let bind_key f = 
    //    match get_key () with
    //    | Some x -> 
    //      maintain_state_with data <| Settings (start, {
    //        state with
    //          focused = false
    //          config = 
    //            {
    //              state.config with
    //                keybinds = f x
    //            }
    //      })
    //    | None -> maintain_state data
    //  let handle_numbers f = 
    //    let is_key_clicked x = data.inputs.kbd_clicks |> Array.exists (fun (y, _, _, _) -> y = x)
    //    match () with
    //    | _ when is_key_clicked Keys.Enter ->
    //      maintain_state_with data <| Settings (start, {
    //        state with
    //          focused = false
    //          config = f (if state.temp_number <> "" then state.temp_number else "0")
    //      })
    //    | _ when is_key_clicked Keys.Escape ->
    //      maintain_state_with data <| Settings (start, {
    //        state with
    //          focused = false
    //      })
    //    | _ ->
    //      let num, x = Keyboard.update_text allowed_num_chars state.temp_number state.settings_selection_x data.inputs
    //      maintain_state_with data <| Settings (start, {
    //        state with
    //          settings_selection_x = x
    //          temp_number = num
    //      })
    //  match state.settings_selection_y with
    //  // screen width
    //  | 0 -> 
    //    handle_numbers (fun x -> {
    //      state.config with 
    //        screen_width = int x
    //    })
    //  // screen height
    //  | 1 -> 
    //    handle_numbers (fun x -> {
    //      state.config with 
    //        screen_height = int x
    //    })
    //  // key up
    //  | 2 -> 
    //    bind_key (fun x -> {
    //      state.config.keybinds with
    //        up = x
    //    })
    //  // key down
    //  | 3 -> 
    //    bind_key (fun x -> {
    //      state.config.keybinds with
    //        down = x
    //    })
    //  // key left
    //  | 4 -> 
    //    bind_key (fun x -> {
    //      state.config.keybinds with
    //        left = x
    //    })
    //  // key right
    //  | 5 -> 
    //    bind_key (fun x -> {
    //      state.config.keybinds with
    //        right = x
    //    })
    //  | _ -> maintain_state data
    //else
    //  // TODO: keybinds validation: duplicates    
    //  match () with
    //  | _ when data.inputs.kbd_confirm ->
    //    match state.settings_selection_y with
    //    | 0 | 1 | 2 | 3 | 4 | 5 ->
    //      maintain_state_with data <| Settings (start, {
    //        state with
    //          focused = true
    //          temp_number = "0"
    //      })
    //    | 6 ->
    //      maintain_state_with data <| Settings (start, {
    //        state with
    //          config =
    //            {
    //              state.config with
    //                username = 
    //                  if state.config.username = ""
    //                  then data.user.Value.username
    //                  else ""
    //                password = 
    //                  if state.config.password = ""
    //                  then data.user.Value.password
    //                  else ""
    //            }
    //      })
    //    // save
    //    | 7 -> 
    //      {
    //        maintain_state data with
    //          states = data.past
    //          config = state.config
    //      }
    //    // cancel
    //    | 8 -> 
    //      {
    //        maintain_state data with
    //          states = data.past
    //      }
    //    | _ -> maintain_state data
    //  | _ when data.inputs.kbd_cancel ->
    //    {
    //      maintain_state data with
    //        states = data.past
    //    }
    //  | _ -> 
    //    let _, _, y = navigate (data.inputs.dx, -data.inputs.dy) (0, state.settings_selection_y) <| List.mapi (fun i x -> ((), 0, i)) setting_opts
    //    maintain_state_with_sounds data <| Settings (start, {
    //      state with
    //        settings_selection_y = y
    //    })
    //    <| (
    //      if y <> state.settings_selection_y
    //      then [{hero = None; sound = "tonk"}]
    //      else []
    //    )

  let draw: game_drawer = fun data ->
    ()
    //let (start, state) = get_state data.present
    //let main = Assets.get_image main
    //// draw background
    //data.sprite_batch.Draw (main, Rectangle(0, 0, Constants.buffer_width, Constants.buffer_height), Color.White)

    //draw_awaiting_response data

    //let draw_str_centered = draw_string_centered Constants.font data 0.8f
    
    //// TODO: zip these together
    //setting_opts
    //|> List.iteri (fun i (v, x, y2) ->
    //  let s = if state.settings_selection_y = i && not state.focused then "> " + v + " <" else v
    //  draw_str_centered (new Vector2 (x, y2)) s
    //)

    //[
    //  if state.focused && state.settings_selection_y = 0 then state.temp_number else string state.config.screen_width
    //  if state.focused && state.settings_selection_y = 1 then state.temp_number else string state.config.screen_height
    //  (string_of_key state.config.keybinds.up).ToUpper ()
    //  (string_of_key state.config.keybinds.down).ToUpper ()
    //  (string_of_key state.config.keybinds.left).ToUpper ()
    //  (string_of_key state.config.keybinds.right).ToUpper ()
    //  if state.config.username <> "" && state.config.password <> "" then "Yes" else "No"
    //]
    //|> List.iteri (fun i x ->
    //  let s = if state.settings_selection_y = i && state.focused then "> " + x + " <" else x
    //  draw_str_centered (new Vector2 (Constants.buffer_width_f32 * 0.7f, Constants.buffer_height_f32 * (0.2f + 0.08f * float32 i))) s
    //)