using Larx.UserInterface.Widgets;

namespace Larx.UserInterface.Components.Modals
{
    public delegate void Confirm();
    public delegate void Submit(string message);
    public delegate void Cancel();

    public interface IModal
    {
        IWidget Component { get; }

        void Submit();

        void Close();
    }
}