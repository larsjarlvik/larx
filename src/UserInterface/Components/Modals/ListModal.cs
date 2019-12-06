using System.Collections.Generic;
using System.Linq;
using Larx.UserInterface.Widgets;

namespace Larx.UserInterface.Components.Modals
{
    public class ListModal : IModal
    {
        private const float width = 250.0f;
        private const string prefix = "list-";
        private const string cancelKey = "CancelButton";
        private readonly Submit submit;
        private readonly Cancel cancel;

        public IWidget Component { get; }

        public ListModal(string title, string actionText, string[] options, Submit submitCallback, Cancel cancelCallback)
        {
            submit = submitCallback;
            cancel = cancelCallback;

            Component = new Wrapper(UiKeys.Modal.Key, new Container("container", Direction.Vertical,
                new List<IWidget> {
                    new Label("title", title, 16.0f),
                    new Container("list-items", Direction.Vertical,
                        options.Select(o => new Button(prefix + o, o, width, ButtonStyle.Default, 1.0f, Align.Left) as IWidget).ToList(),
                        0.0f, -1.0f
                    ),
                    new Button(cancelKey, "Cancel", width, ButtonStyle.Dismiss),
                }, 20.0f, 10.0f
            ));
        }

        public void Update()
        {
            if (Ui.State.Click == null) return;

            if (Ui.State.Click.Key.StartsWith(prefix)) {
                submit(Ui.State.Click.Key.Substring(prefix.Length));
            }

            if (Ui.State.Click.Key == cancelKey) {
                Ui.State.Focused = null;
                cancel();
            }
        }

        public void Submit() { }
    }
}