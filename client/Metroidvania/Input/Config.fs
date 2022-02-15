#nowarn "0025"
#nowarn "0062"
namespace game

open System
open System.IO
open System.Text.RegularExpressions
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework.Graphics
open System.Threading

open Globals
open Globals.Utilities
open Globals.Input
open Globals.Values
open Json

module Config =
  module Constants =
    let installation_path = AppDomain.CurrentDomain.BaseDirectory
    let app_data_path = 
      Environment.GetFolderPath Environment.SpecialFolder.ApplicationData 
      + Constants.file_separator 
      + Constants.dev_name 
      + Constants.file_separator 
      + Constants.game_name 
      + Constants.file_separator
    let config_file = app_data_path + "config.json"
    // TODO: determine if necessary or not since using content pipeline
    let music_path = app_data_path + "music" + Constants.file_separator
    let music_file = music_path + "music.json"
    let log_file = app_data_path + "log.txt"

    module Keywords =
      let skip_opening = "skip_opening"

  module Defaults =
    let defaults =
      Map.ofArray ([|
        ("env", "local")
        (Constants.Keywords.skip_opening, "false")
      |])
      
    let default_binds = [|
      Some <| Keyboard [|
        Keys.Up, Up
        Keys.Down, Down
        Keys.Left, Left
        Keys.Right, Right
        Keys.Z, Confirm
        Keys.X, Cancel
        Keys.C, Detail
        Keys.V, More
      |]
      // | Controller of PlayerIndex * (Buttons * action) array
      None
      None
      None
    |]

    let get = !~ Map.find defaults

    let get_default s x = if Map.containsKey s defaults then defaults.[s] else x

  module File =
    if !- File.Exists Constants.config_file
    then
      ignore <| Directory.CreateDirectory Constants.app_data_path
      use fs = File.Create Constants.config_file
      let json = System.Text.Encoding.UTF8.GetBytes "{}"
      fs.Write (json, 0, json.Length)
      fs.Close ()

    let update_file cfg = 
      let s = "{}"
        // TODO
        //string_of_json <| JObject (List.concat [
        //  [
        //    "keybinds", JObject (Map.ofList [
        //      "up", JString <| string_of_key cfg.keybinds.up
        //      "down", JString <| string_of_key cfg.keybinds.down
        //      "left", JString <| string_of_key cfg.keybinds.left
        //      "right", JString <| string_of_key cfg.keybinds.right
        //    ])
        //    "resolution", JObject (Map.ofList [
        //      "width", JInt cfg.screen_width
        //      "height", JInt cfg.screen_height
        //    ])
        //    "volume", JInt cfg.volume
        //  ]
        //  if cfg.username <> "" && cfg.password <> ""
        //  then 
        //    [
        //      "autologin", JObject (Map.ofList [
        //        "username", JString cfg.username
        //        "password", JString cfg.password
        //      ])
        //    ]
        //  else []
        //]
        //|> Map.ofList)
      File.WriteAllLines (Constants.config_file, [|s|])

    let mutable contents =
      File.ReadAllText Constants.config_file
      |> Parser.run json_parser
      |> get_object

    let has x = Map.containsKey x contents
    let get x = contents.[x]

    let get_default s x = if has s then get s else x

  module CommandLine =
    let command_line =
      Environment.GetCommandLineArgs ()
      |> Array.tail
      |> Array.map (!~ (currify Regex.Match) @"^--(?<key>[a-zA-Z]+)=(?<value>.*)$")
      |> Array.filter (fun m -> m.Success)
      |> Array.map (fun m -> (m.Groups.["key"].Value, m.Groups.["value"].Value))
      |> Map

    let has = command_line.ContainsKey

    let get = !~ Map.find command_line

  let get s =
    match () with
    | _ when CommandLine.has s -> CommandLine.get s
    //| _ when File.has s -> File.get s |> get_string
    | _ -> Defaults.get s // TODO: safe default

  let env = get "env"

  // TODO: move these into settings object
  let mutable window_fullscreen = false
  let mutable window_width = 1024
  let mutable window_height = 768
  let mutable screen_transition = 500
  let mutable mouse_click_radius = 10
  let mutable language = "english"

  let mutable config: config = 
    //let get_property_with_default x f xs json = 
    //  match get_property f xs json with
    //  | Some y -> y
    //  | None -> x
    //let json = JObject File.contents
    //{
    //  keybinds = 
    //    let get_keybind_with_default x y = get_property_with_default x (get_string >> key_of_string) ["keybinds"; y] json
    //    {
    //      confirm = get_keybind_with_default Keys.Enter "confirm"
    //      cancel = get_keybind_with_default Keys.Back "cancel"
    //      inspect = get_keybind_with_default Keys.Space "inspect"
    //      escape = get_keybind_with_default Keys.Escape "escape"
    //      up = get_keybind_with_default Keys.Up "up"
    //      down = get_keybind_with_default Keys.Down "down"
    //      left = get_keybind_with_default Keys.Left "left"
    //      right = get_keybind_with_default Keys.Right "right"
    //    }
    //  screen_height = get_property_with_default window_height get_int ["resolution"; "height"] json
    //  screen_width = get_property_with_default window_width get_int ["resolution"; "width"] json
    //  username = get_property_with_default "" get_string ["autologin"; "username"] json
    //  password = get_property_with_default "" get_string ["autologin"; "password"] json
    //  volume = get_property_with_default 0 get_int ["volume"] json
    //}
    {
      keybinds = Defaults.default_binds
      screen_height = 0
      screen_width = 0
      volume = 0
    }