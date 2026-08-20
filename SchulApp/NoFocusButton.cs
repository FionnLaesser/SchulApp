using System.Windows.Forms;

namespace SchulApp
{
    public class NoFocusButton : Button
    {
        public NoFocusButton()
        {
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
        }

        protected override bool ShowFocusCues => false;

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (Parent is not PingPongMultiplayerGameForm)
            {
                return;
            }

            // The multiplayer header labels occupy the second header row.
            // Keep the Pause/Back buttons in the first row so they are not
            // covered by the Player 2 label.
            Top = 12;
            Height = Math.Max(Height, 30);
            BringToFront();
        }
    }
}
