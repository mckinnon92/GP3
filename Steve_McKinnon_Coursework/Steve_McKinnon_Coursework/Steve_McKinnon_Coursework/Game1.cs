using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Steve_McKinnon_Coursework
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Main 3D model for the space ship
        private Model spaceShip;
        private Matrix[] spaceShipTransforms;

        // The aspect ratio determines how to scale 3d to 2d projection.
        private float aspectRatio;

        // Set the position of the model in world space, and set the rotation.
        private Vector3 mdlPosition = Vector3.Zero;
        private Vector3 mdlRotation = Vector3.Zero;
        private float mdlRot = 0.0f;
        private Vector3 mdlVelocity = Vector3.Zero;

      /*  // create an array of enemy number one
        private Model mdlEnemyOne;
        private Matrix[] mdlEnemyOneTransforms;
        private EnemyOne[] EnemyOneList = new EnemyOne[GameConstants.NumDaleks];*/

        private Random random = new Random();

        private KeyboardState lastState;
        private int hitCount;

        // Set the position of the camera in world space, for our view matrix.
        private Vector3 cameraPosition = new Vector3(0.0f, 3.0f, 100.0f);
        private Matrix viewMatrix;
        private Matrix projectionMatrix;

       

        private void InitializeTransform()
        {
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45), aspectRatio, 1.0f, 350.0f);

        }

        private void MoveModel()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Create some velocity if the right trigger is down.
            Vector3 mdlVelocityAdd = Vector3.Zero;

            // Find out what direction we should be thrusting, using rotation.
           //mdlVelocityAdd.X = -(float)Math.Sin(mdlRotation);
            //mdlVelocityAdd.Z = -(float)Math.Cos(mdlRotation);

            mdlVelocityAdd.X = 2.0f;
          //  mdlVelocityAdd.Z = 2.0f;

            mdlRotation.X = -(float)Math.Sin(mdlRot);
            mdlRotation.Y = -(float)Math.Cos(mdlRot);
            



            if (keyboardState.IsKeyDown(Keys.A))
            {
                // Rotate left.
                // Create some velocity if the right trigger is down.
                // Now scale our direction by how hard the trigger is down.
                mdlVelocityAdd.X *= -0.05f;
                mdlVelocity += mdlVelocityAdd;

                if (mdlRot <= 0.20f)
                {
                    mdlRot -= -1.0f * 0.20f;
                }
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                // Rotate left.
                // Create some velocity if the right trigger is down.
                // Now scale our direction by how hard the trigger is down.
                mdlVelocityAdd.X *= 0.05f;
                mdlVelocity += mdlVelocityAdd;

                if (mdlRot >= -0.20f)
                {
                    mdlRot -= 1.0f * 0.20f;
                }

            }

            if (keyboardState.IsKeyUp(Keys.A)&&(keyboardState.IsKeyUp(Keys.D)))
            {
                mdlRot = 0.0f;
                if (mdlPosition.X > 0.0f || mdlPosition.X < 0.0f)
                {
                    mdlPosition = Vector3.Zero;
                }
            }

            lastState = keyboardState;
        }

        private Matrix[] SetupEffectTransformDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = projectionMatrix;
                    effect.View = viewMatrix;
                }
            }
            return absoluteTransforms;
        }

        public void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms)
        {
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = false;
            Window.Title = "Space: Because It's Easier Than Land 2";
            InitializeTransform();
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            // TODO: use this.Content to load your game content here
            spaceShip = Content.Load<Model>(".\\Models\\space_frigate_6");
            spaceShipTransforms = SetupEffectTransformDefaults(spaceShip);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            MoveModel();
            // TODO: Add your update logic here
            // Add velocity to the current position.
            mdlPosition += mdlVelocity;

            // Bleed off velocity over time.
            mdlVelocity *= 0.95f;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            Matrix modelTransform = Matrix.CreateRotationZ(mdlRot) * Matrix.CreateTranslation(mdlPosition);
            DrawModel(spaceShip, modelTransform, spaceShipTransforms);

            base.Draw(gameTime);
        }
    }
}
