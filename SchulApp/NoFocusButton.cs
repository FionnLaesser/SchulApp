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
    }
}