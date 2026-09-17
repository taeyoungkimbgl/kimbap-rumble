using Assets.Scripts.Common.UI.Base;
using Scripts.Features.GameScene.HomeScreen.Components;

namespace Scripts.Features.GameScene.HomeScreen
{
    public class HomeScreenView : BaseView
    {
        public HomeScreenButton GetButton(HomeScreenButtonType type)
        {
            return GetUI<HomeScreenButton, HomeScreenButtonType>(type);
        }

        public HomeScreenText GetText(HomeScreenTextType type)
        {
            return GetUI<HomeScreenText, HomeScreenTextType>(type);
        }

        public HomeScreenImage GetImage(HomeScreenImageType type)
        {
            return GetUI<HomeScreenImage, HomeScreenImageType>(type);
        }
    }
}
