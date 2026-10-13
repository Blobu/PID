using Framework.Model;
using Framework.ViewModel.ColorLevels;

namespace Framework.Services
{
    public interface IWindowService
    {
        void ShowMagnifier();
        void ShowColorLevels(ColorLevelsType type);
        void ShowHistogram(ImageCanvasType canvas);
        void UpdateSelectedPoint();
        void CloseAllChildWindows();

        bool IsMagnifierOpen { get; }
        bool IsRowColorLevelsOpen { get; }
        bool IsColumnColorLevelsOpen { get; }
    }
}
