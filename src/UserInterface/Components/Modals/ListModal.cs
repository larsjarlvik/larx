using System.Collections.Generic;
using System.Linq;
using Larx.UserInterface.Widgets;

namespace Larx.UserInterface.Components.Modals
{
    public class ListModal : IModal
    {
        private const float width = 250.0f;
        private readonly Submit submit;

        public IWidget Component { get; }

        public ListModal(string title, string actionText, string[] options, Submit submitCallback)
        {
            submit = submitCallback;

            Component = new Wrapper(UiKeys.Modal.Key, new Container("container", Direction.Vertical,
                new List<IWidget> {
                    new Label("title", title, 16.0f),
                    new Container("list-items", Direction.Vertical,
                        options.Select(o => new Button($"{UiKeys.Modal.Submit}-{o}", o, width, ButtonStyle.Default, 1.0f, Align.Left) as IWidget).ToList(),
                        0.0f, -1.0f
                    ),
                    new Button(UiKeys.Modal.Cancel, "Cancel", width, ButtonStyle.Dismiss),
                }, 20.0f, 10.0f
            ));
        }

        public void Submit()
        {
            submit(Ui.State.Click.Key.Substring(UiKeys.Modal.Submit.Length + 1));
            Ui.State.Focused = null;
        }

        public void Close()
        {
            Ui.State.Focused = null;
        }
    }
}