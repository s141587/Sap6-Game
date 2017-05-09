﻿using System.Threading;
using EngineName;
using EngineName.Components;
using EngineName.Components.Renderable;
using EngineName.Logging;
using EngineName.Systems;
using EngineName.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameName.Scenes
{
    public class ChatScene : Scene
    {
        public override void Draw(float t, float dt)
        {
            Game1.Inst.GraphicsDevice.Clear(Color.Aqua);
            base.Draw(t, dt);
        }

        public override void Init()
        {

            AddSystems(
                new FpsCounterSystem(updatesPerSec: 10),
                new NetworkSystem(),
                new Rendering2DSystem(),
                new InputSystem(),
                new ChatSystem()
            );

#if DEBUG
            AddSystem(new DebugOverlay());
#endif

            base.Init();

            int player = AddEntity();
            AddComponent(player, new CInput());
            AddComponent(player, new CTransform() { Position = new Vector3(0, -40, 0), Scale = new Vector3(1f) });
            AddComponent<C2DRenderable>(player, new CText()
            {
                font = Game1.Inst.Content.Load<SpriteFont>("Fonts/DroidSans"),
                format = "Type Here",
                color = Color.White,
                position = new Vector2(300, 750),
                origin = Vector2.Zero
            });

            AddComponent<C2DRenderable>(AddEntity(), new CText()
            {
                font = Game1.Inst.Content.Load<SpriteFont>("Fonts/DroidSans"),
                format = "0 peers",
                color = Color.White,
                position = new Vector2(300, 20),
                origin = Vector2.Zero
            });

            int eid = AddEntity();
      
            AddComponent<C2DRenderable>(eid, new CSprite
            {
                texture = Game1.Inst.Content.Load<Texture2D>("Textures/clubbing"),
                position = new Vector2(300, 300),
                color = Color.White
            });



            //new Thread(NewThread).Start();
            Log.Get().Debug("TestScene initialized.");
        }
       
        static void NewThread()
        {
            var test = new NetworkSystem();
            test.Bot();
        }
    }
}