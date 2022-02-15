//#nowarn "62"

namespace game
            
//open System
//open System.IO            
//open System.Threading
//open Microsoft.Xna.Framework
//open Microsoft.Xna.Framework.Audio
//open Microsoft.Xna.Framework.Content
//open Microsoft.Xna.Framework.Media

//open Globals
//open Config
//open Parser
//open Json
//open Logging

//module Sound =
//  type Song_id = string
//  type Song_state = string

//  let initial_mood = "mood"

//  let opening = "opening"
//  let menu = "menu"
//  let battle = "battle"
//  let states = [opening; menu; battle]

//  let mutable music_volume = 100
//  let mutable sfx_volume = 100

//  // SoundEffect.MasterVolume = 1.0f;

//  module Music =
//    type Song = {
//      name: string
//      mood: string * string
//      length: int
//      path: string
//      state: string
//    }
  
//    let music =
//      if File.Exists Constants.music_file
//      then
//        File.ReadAllText Constants.music_file
//        |> Parser.run json_parser
//        |> get_object
//        |> Map.map (fun _ json -> 
//          {
//            name = get_string_value "name" json
//            mood = 
//              let start = get_value "mood" >> get_string_value "start" <| json
//              let ending = get_value "mood" >> get_string_value "end" <| json
//              start, ending
//            length = 0
//            path = get_string_value "path" json
//            state = get_string_value "state" json
//          }
//        )
//      else Map.empty

//    let rng = new System.Random ()

//    let pick_random_song music = 
//      let arr = Map.toArray music
//      fst arr.[rng.Next (arr.Length)]

//    let mutable songs: (Song_id, Media.Song) Map = Map.empty
//    let mutable next: (Song_state, Song_id) Map = Map.empty
//    let mutable now_playing = ""
//    let mutable ended = false
//    let mutable state = ""

//    let mutable fade = 100

//    let set_song_volume () = MediaPlayer.Volume <- float32 (music_volume * fade) * 0.0001f

//    let pick_next_song state mood =
//      pick_random_song
//      <| List.fold (fun a x -> 
//        let filtered = Map.filter x a
//        if Map.isEmpty filtered then a else filtered
//      ) music [
//        fun _ v -> v.state = state
//        fun _ v -> now_playing <> "" && fst v.mood = mood
//      ]

//    let pick_next_songs mood = 
//      states
//      |> List.map (fun state -> state, pick_next_song state mood)
//      |> Map.ofList

//    let load id = content_manager.Load<Media.Song> (Constants.music_path + music.[id].path)

//    let init () =
//      set_song_volume ()
//      let song = pick_next_song opening initial_mood
//      songs <- Map.ofList [song, content_manager.Load<Media.Song> (Constants.music_path + music.[song].path)]
//      MediaPlayer.Play songs.[song]
//      next <- pick_next_songs <| snd music.[song].mood
//      let play_next_song _ =
//        MediaPlayer.Play songs.[next.[state]]
//        let next' = pick_next_songs state 
//        let (free, keep) =
//          songs
//          |> Map.partition (fun k _ -> k <> now_playing && !-- Map.exists (fun _ v -> v = k) next)
//        free
//        |> Map.iter (fun _ v -> v.Dispose ())
//        songs <-
//          let added = 
//            next'
//            |> Map.toList
//            |> List.map snd
//            |> List.distinct
//            |> List.filter (fun v -> !-- Map.exists (fun k _ -> k = v) keep)
//            |> List.map (fun v -> v, load v)
//          Map.ofList (List.append (Map.toList keep) added)
//        next <- next'
//      MediaPlayer.MediaStateChanged.Add play_next_song

//    let update (content: ContentManager) time (state': state) =
//      // TODO: update volume
//      // TODO: allow for force picking songs for song menu
//      state <- 
//        match state' with
//        | Opening _ -> opening
//        | Login _ -> menu
//        | Main _ -> menu
//        | Settings _ -> menu
//        | Deckbuilder _ -> menu
//        | Stages _ -> menu
//        | Lobby _ -> menu
//        | Battle _ -> battle

//  module Effects =
//    // is there a way to stop playing effects? probably
//    // read in request sounds from event loop
//    //  cancel playing sounds of the same name?
//    //   map?



//    // effects stored in assets map and heroes objects

//    let play_sound (cfg : Sound_config) : unit =
//      let sfx =
//        let sfx =
//          if Option.isNone cfg.hero
//          then Assets.get_sound cfg.sound
//          else (Heroes.get cfg.hero.Value).sounds.[cfg.sound]
//        sfx.CreateInstance ()
//      sfx.Volume <- float32 sfx_volume * 0.01f
//      sfx.Play ()

      

//    ()

//    let play_sounds = List.iter play_sound


//  // sound effects flow
//  // song flow
//  // volume
  
  
//  // TODO: song flow

