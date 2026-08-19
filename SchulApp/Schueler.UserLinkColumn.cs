using Microsoft.EntityFrameworkCore;
using SchulApp.Data;

namespace SchulApp
{
    public partial class Schueler
    {
        private bool benutzerSpalteWirdGeladen;

        protected override void OnLoad(EventArgs e)
        {
            studentGrid.DataBindingComplete +=
                studentGrid_DataBindingComplete;

            base.OnLoad(e);
        }

        private async void studentGrid_DataBindingComplete(
            object? sender,
            DataGridViewBindingCompleteEventArgs e)
        {
            if (benutzerSpalteWirdGeladen || IsDisposed)
            {
                return;
            }

            benutzerSpalteWirdGeladen = true;

            try
            {
                if (studentGrid.Columns["Benutzerkonto"] == null)
                {
                    DataGridViewTextBoxColumn userColumn =
                        new DataGridViewTextBoxColumn
                        {
                            Name = "Benutzerkonto",
                            HeaderText = "Benutzer",
                            ReadOnly = true,
                            Width = 150
                        };

                    studentGrid.Columns.Add(userColumn);
                }

                await using SchulAppContext db = new SchulAppContext();

                Dictionary<int, string> userByStudentId = await db.Benutzer
                    .AsNoTracking()
                    .Where(x => x.SchuelerId.HasValue)
                    .Select(x => new
                    {
                        SchuelerId = x.SchuelerId!.Value,
                        x.Benutzername
                    })
                    .ToDictionaryAsync(
                        x => x.SchuelerId,
                        x => x.Benutzername
                    );

                foreach (DataGridViewRow row in studentGrid.Rows)
                {
                    if (row.IsNewRow ||
                        row.Cells["SchuelerId"].Value == null)
                    {
                        continue;
                    }

                    int schuelerId = Convert.ToInt32(
                        row.Cells["SchuelerId"].Value
                    );

                    row.Cells["Benutzerkonto"].Value =
                        userByStudentId.TryGetValue(
                            schuelerId,
                            out string? benutzername
                        )
                            ? benutzername
                            : "Kein Benutzer";
                }
            }
            catch (Exception)
            {
                if (studentGrid.Columns["Benutzerkonto"] != null)
                {
                    foreach (DataGridViewRow row in studentGrid.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            row.Cells["Benutzerkonto"].Value =
                                "Nicht verfügbar";
                        }
                    }
                }
            }
            finally
            {
                benutzerSpalteWirdGeladen = false;
            }
        }
    }
}
