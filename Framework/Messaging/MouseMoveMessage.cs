using System.Windows;
using Framework.Model;

namespace Framework.Messaging
{
    public class MouseMoveMessage
    {
        public Point Position { get; }
        public ImageCanvasType CanvasType { get; }

        public MouseMoveMessage(Point position, ImageCanvasType canvasType)
        {
            Position = position;
            CanvasType = canvasType;
        }
    }
}