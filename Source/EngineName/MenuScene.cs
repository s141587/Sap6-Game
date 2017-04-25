namespace EngineName {

//--------------------------------------
// USINGS
//--------------------------------------

using System;
using System.Collections.Generic;

using Components;
using Components.Renderable;
using Logging;
using Systems;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//--------------------------------------
// CLASSES
//--------------------------------------

// TODO: Might be more sane to move this class to somewhere else eventually.

/// <summary>Provides a base scene implementation for menus.</summary>
public abstract class MenuScene: Scene {
    //--------------------------------------
    // PUBLIC PROPERTIES
    //--------------------------------------

    /// <summary>Gets or sets the font used to render menu text.</summary>
    public SpriteFont Font { get; set; }

    /// <summary>Gets or sets the key used to move up amongst the menu items.</summary>
    public Keys MoveUpKey { get; set; } = Keys.Up;

    /// <summary>Gets or sets the key used to move down amongst the menu items.</summary>
    public Keys MoveDownKey { get; set; } = Keys.Down;

    /// <summary>Gets or sets the key used to select the highlighted menu item.</summary>
    public Keys SelectKey   { get; set; } = Keys.Enter;

    //--------------------------------------
    // NESTED TYPES
    //--------------------------------------

    /// <summary>Represents a menu item.</summary>
    private class MenuItem {
        /// <summary>The callback to invoke when the item is activated.</summary>
        public Action Select;

        /// <summary>The text component used to render the item.</summary>
        public CText Text;
    }

    //--------------------------------------
    // NON-PUBLIC FIELDS
    //--------------------------------------

    /// <summary>Indicates whether the selection can be changed in the menu. Used to prevent
    ///          selection spamming.</summary>
    private bool mCanMove = true;

    /// <summary>The item that have been added to the menu.</summary>
    private readonly List<MenuItem> mItems = new List<MenuItem>();

    /// <summary>The index of the currently selceted menu item.</summary>
    private int mSelIndex;

    /// <summary>The menu selection highlight component used to render the selection highlight (e.g.
    ///          an arrow pointing to the currently highlighted item).</summary>
    private CText mSelHighlight;

    //--------------------------------------
    // PUBLIC METHODS
    //--------------------------------------

    /// <summary>Initializes the menu.</summary>
    public override void Init() {
        AddSystems(new Rendering2DSystem(),
                   new FpsCounterSystem(updatesPerSec: 10));

        base.Init();

        Font = Game1.Inst.Content.Load<SpriteFont>("Fonts/DroidSans");

        AddComponent<C2DRenderable>(AddEntity(), mSelHighlight = new CText {
            color    = Color.Black,
            font     = Game1.Inst.Content.Load<SpriteFont>("Fonts/DroidSans"),
            format   = "--->",
            origin   = Vector2.Zero,
            position = new Vector2(150, 0)
        });

        // TODO: Renndering2DSystem refuses to draw text without this hack.
        AddComponent(AddEntity(), new CCamera {});
    }

    /// <summary>Performs draw logic (and, in the case of the <see cref="MenuScene"/> class, some
    ///          update logic, because we only need to do it once per frame.)</summary>
    /// <param name="t">The total game time, in seconds.</param>
    /// <param name="dt">The game time, in seconds, since the last call to this method.</param>
    public override void Draw(float t, float dt) {
        base.Draw(t, dt);

        var keyboard = Keyboard.GetState();
        var canMove  = true;

        if (keyboard.IsKeyDown(MoveUpKey)) {
            if (mCanMove) {
                mSelIndex -= 1;
                if (mSelIndex < 0) {
                    mSelIndex = mItems.Count - 1;
                }
            }

            canMove = false;
        }

        if (keyboard.IsKeyDown(MoveDownKey)) {
            if (mCanMove) {
                mSelIndex += 1;
                if (mSelIndex >= mItems.Count) {
                    mSelIndex = 0;
                }
            }

            canMove = false;
        }

        if (keyboard.IsKeyDown(SelectKey)) {
            if (mCanMove) {
                var s = mItems[mSelIndex].Text.format;
                Log.Get().Debug($"Selecting menu item: {s}");
                mItems[mSelIndex].Select();
            }

            canMove = false;
        }

        mCanMove = canMove;
        mSelHighlight.position.Y = mItems[mSelIndex].Text.position.Y;
    }

    //--------------------------------------
    // NON-PUBLIC METHODS
    //--------------------------------------

    protected void CreateLabel(string text, Action cb, Color? color=null) {
        // TODO: Super messy solution but it's ok for now. Need better positioning of items.

        var x = 300;
        var y = 100;

        if (mItems.Count > 0) {
            y = (int)mItems[mItems.Count - 1].Text.position.Y + 30;
        }

        var label = new MenuItem {
            Select = cb,
            Text       = new CText {
                color    = color ?? Color.Black,
                font     = Game1.Inst.Content.Load<SpriteFont>("Fonts/DroidSans"),
                format   = text,
                origin   = Vector2.Zero,
                position = new Vector2(x, y)
            }
        };

        AddComponent<C2DRenderable>(AddEntity(), label.Text);
        mItems.Add(label);
    }
}

}
