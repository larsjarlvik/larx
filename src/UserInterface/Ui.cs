using System;
using System.Collections.Generic;
using System.Linq;
using Larx.UserInterface.Components;
using Larx.UserInterface.Widgets;
using OpenTK;

namespace Larx.UserInterface
{
    public class Ui
    {
        private Matrix4 pMatrix;
        public static readonly UiState State;
        private readonly Dock page;
        private readonly MainMenu mainMenu;
        private readonly RightMenu rightMenu;
        private readonly ApplicationInfo applicationInfo;
        private const float uiScale = 1.0f;
        private InputModal modal;

        static Ui()
        {
            State = new UiState();
        }

        public Ui()
        {
            mainMenu = new MainMenu();
            rightMenu = new RightMenu();
            applicationInfo = new ApplicationInfo();

            page = new Dock("page", new Vector2(1280, 720), new List<Child>() {
                new Child(DockPosition.BottomLeft, mainMenu.Component),
                new Child(DockPosition.BottomRight, rightMenu.Component),
                new Child(DockPosition.TopLeft, applicationInfo.Component),
            });
        }

        public bool Update()
        {
            State.MouseRepeat = State.MousePressed;
            State.Hover = page.Intersect(Larx.State.Mouse.Position * uiScale, new Vector2(0.0f, 0.0f));
            State.Click = State.MousePressed && !Larx.State.Mouse.LeftButton ? State.Hover : null;
            State.MousePressed = Larx.State.Mouse.LeftButton;

            if (State.MousePressed) State.Focused = State.Hover != null ? State.Hover : null;

            applicationInfo.Update();

            if (modal != null) {
                modal.Update();
                return true;
            }

            mainMenu.Update();
            rightMenu.Update();

            State.Click = null;
            return true;
        }

        public void Render()
        {
            page.Render(pMatrix, new Vector2(0, 0));
        }

        public void KeyPress(Char key)
        {
            if (key == 49 && modal != null) {
                modal.Submit();
            }

            var active = State.Focused as TextBox;
            if (active != null) active.KeyPress(key);
        }

        public void Resize()
        {
            pMatrix = Matrix4.CreateOrthographicOffCenter(0, Larx.State.Window.Size.Width * uiScale, Larx.State.Window.Size.Height * uiScale, 0f, 0f, -1f);
            page.Size = new Vector2(Larx.State.Window.Size.Width * uiScale, Larx.State.Window.Size.Height * uiScale);
        }

        public void ShowModal(string title, string actionText, Submit submitCallback)
        {
            modal = new InputModal(title, actionText, submitCallback, () => CloseModals());
            page.Children.Add(new Child(DockPosition.Center, modal.Component));
        }

        public void CloseModals()
        {
            modal = null;
            page.Children.RemoveAll(x => x.Component.Key == UiKeys.Modal.Key);
        }
    }
}