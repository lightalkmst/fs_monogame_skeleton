﻿module PlatformerGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type Game1 () as x =
   inherit Game()

   do x.Content.RootDirectory <- "Content"
   let graphics = new GraphicsDeviceManager(x)
   let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>

   override x.Initialize() =
       do spriteBatch         
       do base.Initialize()
       ()

   override x.LoadContent() =
       ()

   override x.Update (gameTime) =
       ()

   override x.Draw (gameTime) =
       do x.GraphicsDevice.Clear Color.CornflowerBlue
       ()

let main argv =
   use g = new Game1()
   g.Run()
   System.Console.ReadLine () |> ignore
   0

main [||] |> ignore