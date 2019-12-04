using System;
using System.Collections.Generic;
using Larx.UserInterface.Widgets;

namespace Larx.UserInterface.Components
{
    public delegate void Submit(string message);
    public delegate void Cancel();

    public class InputModal
    {
        private const string submitKey = "SubmitButton";
        private const string cancelKey = "CancelButton";
        private readonly Submit submit;
        private readonly Cancel cancel;
        private readonly TextBox input;

        public Wrapper Component { get; }

        public InputModal(string title, string actionText, Submit submitCallback, Cancel cancelCallback)
        {
            submit = submitCallback;
            cancel = cancelCallback;
            input = new TextBox("input", 200.0f);

            Component = new Wrapper(UiKeys.Modal.Key, new Container("container", Direction.Vertical,
                new List<IWidget> {
                    new Label("title", title, 16.0f),
                    input,
                    new Container("actions", Direction.Horizonal, new List<IWidget> {
                        new Button(cancelKey, "Cancel", 95.0f, ButtonStyle.Dismiss),
                        new Button(submitKey, actionText, 95.0f),
                    }, 0.0f, 10.0f)
                }, 20.0f, 10.0f
            ));

            Ui.State.Focused = input;
        }

        public void Update()
        {
            if (Ui.State.Click == null) return;

            if (Ui.State.Click.Key == submitKey && !string.IsNullOrWhiteSpace(input.Text)) {
                Submit();
            }

            if (Ui.State.Click.Key == cancelKey) {
                Ui.State.Focused = null;
                cancel();
            }
        }

        public void Submit()
        {
            Ui.State.Focused = null;
            submit(input.Text);
        }
    }
}