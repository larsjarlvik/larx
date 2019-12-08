using System;
using System.Collections.Generic;
using Larx.UserInterface.Widgets;
using OpenTK;

namespace Larx.UserInterface.Components.Modals
{
    public class ConfirmModal : IModal
    {
        private readonly Confirm confirm;
        private readonly Cancel cancel;
        public IWidget Component { get; }

        public ConfirmModal(string title, string confirmText, Confirm confirmCallback, Cancel cancelCallback = null)
        {
            confirm = confirmCallback;
            cancel = cancelCallback;

            Component = new Wrapper(UiKeys.Modal.Key, new Container("container", Direction.Vertical,
                new List<IWidget> {
                    new Label("title", title, 16.0f),
                    new Label("confirm", confirmText, 14.0f, 200.0f),
                    new Container("actions", Direction.Horizonal, new List<IWidget> {
                        new Button(UiKeys.Modal.Cancel, "No", 95.0f, ButtonStyle.Dismiss),
                        new Button(UiKeys.Modal.Submit, "Yes", 95.0f, ButtonStyle.Action),
                    }, 0.0f, 10.0f)
                }, 20.0f, 10.0f
            ));
        }

        public void Submit()
        {
            Ui.State.Focused = null;
            confirm();
        }

        public void Close()
        {
            Ui.State.Focused = null;
            if (cancel != null) cancel();
        }
    }
}