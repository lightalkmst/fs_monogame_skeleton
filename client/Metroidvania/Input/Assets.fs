#nowarn "62"

namespace game
            
open System
open System.IO            
open System.Threading
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Audio

open Globals
open Globals.Utilities
open Globals.Values
open Logging

module Assets =
  type assets = (string, content) Map

  type Loaders = {
    image: string -> Texture2D
    font: string -> SpriteFont
    text: string -> string
    sound: string -> SoundEffect
  }

  let dir = Config.Constants.app_data_path

  let assets: assets ref = ref Map.empty

  let load_asset loaders file =
    // files are expected to follow the naming convention <name>.<type>.xnb
    let existing_files = Directory.GetFiles dir
    try
      // TODO: remove dynamic detection
      let info =
        existing_files
        |> Array.map (fun s -> s.Substring dir.Length)
        |> Array.find (fun s -> s.StartsWith (file + "."))
        |> (fun s -> s.Split [|'.'|])
      let name = info.[0] + "." + info.[1]
      let asset = 
        match info.[1] with
        | "png" -> Image (loaders.image name)
        | "spritefont" -> Font (loaders.font name)
        | "json" -> Text (loaders.text name)
        | "wav" -> Sound (loaders.sound name)
        | _ -> failwith ("Assets.load_assets: Unsupported file type: " + info.[1])
      lock assets (fun _ -> assets := Map.add file asset !assets)
    with
    | :? System.ArgumentException as e ->
      log ("Assets.load_assets: file " + file + " does not exist")
      log (e.Message)
      ()

  let bg_load_assets loaders = 
    List.filter (fun file -> !- (!~ Map.containsKey !assets) file)
    >> List.iter (fun file ->
      lock assets (fun _ -> assets := Map.add file Loading !assets)
      ignore <| ThreadPool.QueueUserWorkItem (fun _ -> load_asset loaders file)
    )

  let is_loaded () = Map.forall (fun k v -> v <> Loading) !assets

  // TODO: remove
  let are_loaded = List.forall (fun s -> Map.containsKey s !assets && Map.find s !assets <> Loading)

  let get_text x = 
    match Map.find x !assets with
    | Text x -> x
    | _ -> failwith "Not a text"

  let get_image x = 
    match Map.find x !assets with
    | Image x -> x
    | _ -> failwith "Not an image"

  let get_model x = 
    match Map.find x !assets with
    | Model x -> x
    | _ -> failwith "Not a model"

  let get_font x =
    match Map.find x !assets with
    | Font x -> x
    | _ -> failwith "Not a font"
    
  let get_sound x =
    match Map.find x !assets with
    | Sound x -> x
    | _ -> failwith "Not a sound"