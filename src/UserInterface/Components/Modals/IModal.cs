using Larx.UserInterface.Widgets;

namespace Larx.UserInterface.Components.Modals
{
    public interface IModal
    {
        IWidget Component { get; }

        void Update();

        void Submit();
    }
}