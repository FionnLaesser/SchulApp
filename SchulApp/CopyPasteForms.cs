using System;
using System.Windows.Forms;

namespace SchulApp
{
    public partial class CopyPasteForms : CustomForm
    {
        public CopyPasteForms()
        {
            InitializeComponent();

            ThemeManager.Anwenden(this);
        }

        private void CopyPasteForms_Load(object sender, EventArgs e)
        {
            // Code, der beim Öffnen ausgeführt werden soll
        }
    }
}