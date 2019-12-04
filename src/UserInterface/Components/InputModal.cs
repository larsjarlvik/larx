using System.Collections.Generic;
using Larx.UserInterface.Widgets;

namespace Larx.UserInterface.Components
{
    public delegate void Submit(string message);

    public class InputModal
    {
        private const string submitKey = "SubmitButton";
        private readonly Submit submit;
        private readonly TextBox input;

        public Wrapper Component { get; }

        public InputModal(string title, string actionText, Submit submitCallback)
        {
            submit = submitCallback;
            input = new TextBox("input", 200.0f);

            Component = new Wrapper(UiKeys.Modal.Key, new Container("container", Direction.Vertical,
                new List<IWidget> {
                    new Label("title", title, 16.0f),
                    input,
                    new Button(submitKey, actionText, 200.0f)
                }, 20.0f, 10.0f
            ));

            Ui.State.Focused = input;
        }

        public void Update()
        {
            if (submitKey == Ui.State.Hover?.Key && Ui.State.MousePressed)
                submit(input.Text);
        }
    }
}