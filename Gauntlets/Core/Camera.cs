using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxAwesomeEngine.Core
{
    class Camera : Entity
    {
        private static GraphicsDeviceManager GDeviceManager;
        private Matrix viewMatrix;

        public static Camera Main { get; private set; } = null;
        public Viewport ViewPort { get; private set; }
        public float Zoom { get; set; } = 1.0f;
        public Color BackgroundColor { get; set; } = Color.Gray;

        internal static void InitCamera(GraphicsDeviceManager device)
        {
            GDeviceManager = device;
        }

        public Camera()
        {
            ViewPort = GDeviceManager.GraphicsDevice.Viewport;

            if (Main == null)
                Main = this;
        }

        public Vector2 GetCameraPosition()
        {
            return new Vector2(-Transform.Position.X , -Transform.Position.Y );
        }

        public Vector2 GetViewportSize()
        {
            return new Vector2(ViewPort.Width, ViewPort.Height);
        }

        public Matrix GetMatrix()
        {

            viewMatrix = Matrix.CreateTranslation(GetCameraPosition().X, GetCameraPosition().Y, 0.0f) 
                * Matrix.CreateRotationZ(Utils.Deg2Rad(Transform.Rotation)) 
                * Matrix.CreateScale(Zoom) 
                * Matrix.CreateTranslation(GetViewportSize().X * 0.5f, GetViewportSize().Y * 0.5f, 0.0f);

            return viewMatrix ;
        }
    }
}
