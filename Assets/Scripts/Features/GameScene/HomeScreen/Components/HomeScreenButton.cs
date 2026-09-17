using Assets.Scripts.Common.UI.Base;

namespace Scripts.Features.GameScene.HomeScreen.Components
{
    public class HomeScreenButton : BaseButton<HomeScreenButtonType>
    {
    }
    public enum HomeScreenButtonType
    {
        Rumble = 0,
        Album = 1,
        Settings = 2,
        Weapon = 3,
        Shield = 4,
        Accessory = 5
    }
}
