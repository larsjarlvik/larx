using System;
using System.Collections.Generic;
using Larx.UserInterface.Widgets;

namespace Larx.UserInterface.Components.Modals
{
    public class InputModal : IModal
    {
        private readonly Submit submit;
        private readonly TextBox input;
        public IWidget Component { get; }

        public InputModal(string title, string actionText, string defaultValue, Submit submitCallback)
        {
            submit = submitCallback;
            input = new TextBox("input", 200.0f, defaultValue);

            Component = new Wrapper(UiKeys.Modal.Key, new Container("container", Direction.Vertical,
                new List<IWidget> {
                    new Label("title", title, 16.0f),
                    input,
                    new Container("actions", Direction.Horizonal, new List<IWidget> {
                        new Button(UiKeys.Modal.Cancel, "Cancel", 95.0f, ButtonStyle.Dismiss),
                        new Button(UiKeys.Modal.Submit, actionText, 95.0f, ButtonStyle.Action),
                    }, 0.0f, 10.0f)
                }, 20.0f, 10.0f
            ));

            Ui.State.Focused = input;
        }

        public void Submit()
        {
            if (string.IsNullOrWhiteSpace(input.Text)) return;

            Ui.State.Focused = null;
            submit(input.Text);
        }

        public void Close()
        {
            Ui.State.Focused = null;
        }
    }
}