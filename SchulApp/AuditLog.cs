using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;
using System.ComponentModel;
using System.Text.Json;

namespace SchulApp
{
    public partial class AuditLog : CustomForm
    {
        private const int AutoRefreshIntervalMilliseconds = 1000;

        private readonly System.Windows.Forms.Timer autoRefreshTimer;
        private bool wirdGeladen;

        public AuditLog()
        {
            InitializeComponent();

            autoRefreshTimer = new System.Windows.Forms.Timer
            {
                Interval = AutoRefreshIntervalMilliseconds
            };
            autoRefreshTimer.Tick += AutoRefreshTimer_Tick;
            FormClosed += AuditLog_FormClosed;

            // Im Visual-Studio-Designer keine Session-/Admin-Prüfung ausführen
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            if (!string.Equals(
                    BenutzerSession.Rolle,
                    LoginBenutzer.RolleAdmin,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException(
                    "Nur Administratoren dürfen das Audit Log öffnen."
                );
            }

            ThemeManager.Anwenden(this);
        }

        private async void AuditLog_Shown(object? sender, EventArgs e)
        {
            await AuditLogsLadenAsync();
            autoRefreshTimer.Start();
        }

        private async void reloadButton_Click(object? sender, EventArgs e)
        {
            await AuditLogsLadenAsync();
        }

        private async void AutoRefreshTimer_Tick(object? sender, EventArgs e)
        {
            await AuditLogsLadenAsync(showError: false);
        }

        private void AuditLog_FormClosed(object? sender, FormClosedEventArgs e)
        {
            autoRefreshTimer.Stop();
        }

        private async Task AuditLogsLadenAsync(bool showError = true)
        {
            if (wirdGeladen)
            {
                return;
            }

            wirdGeladen = true;
            reloadButton.Enabled = false;

            int? selectedId =
                auditGrid.CurrentRow?.DataBoundItem is AuditLogRow selectedRow
                    ? selectedRow.Id
                    : null;

            DataGridViewColumn? sortColumn = auditGrid.SortedColumn;
            SortOrder sortOrder = auditGrid.SortOrder;

            try
            {
                await using SchulAppContext db = new SchulAppContext();

                List<AuditLogRow> rows =
                    await db.AuditLogs
                        .AsNoTracking()
                        .OrderByDescending(x => x.TimestampUtc)
                        .Take(500)
                        .Select(x => new AuditLogRow
                        {
                            Id = x.Id,
                            Zeit = x.TimestampUtc,
                            Benutzer = x.UserName,
                            Rolle = x.UserRole,
                            Aktion = x.Action,
                            Entitaet = x.EntityType,
                            EntityId = x.EntityId,
                            Quelle = x.Source,
                            Changes = x.Changes
                        })
                        .ToListAsync();

                auditGrid.DataSource = rows;

                if (auditGrid.Columns[nameof(AuditLogRow.Changes)]
                    is DataGridViewColumn changesColumn)
                {
                    changesColumn.Visible = false;
                }

                if (auditGrid.Columns[nameof(AuditLogRow.Id)]
                    is DataGridViewColumn idColumn)
                {
                    idColumn.FillWeight = 35;
                }

                if (auditGrid.Columns[nameof(AuditLogRow.Zeit)]
                    is DataGridViewColumn zeitColumn)
                {
                    zeitColumn.FillWeight = 90;
                }

                if (sortColumn != null &&
                    auditGrid.Columns.Contains(sortColumn.Name) &&
                    sortOrder != SortOrder.None)
                {
                    auditGrid.Sort(
                        auditGrid.Columns[sortColumn.Name],
                        sortOrder == SortOrder.Descending
                            ? ListSortDirection.Descending
                            : ListSortDirection.Ascending
                    );
                }

                if (selectedId.HasValue)
                {
                    foreach (DataGridViewRow gridRow in auditGrid.Rows)
                    {
                        if (gridRow.DataBoundItem is AuditLogRow row &&
                            row.Id == selectedId.Value)
                        {
                            gridRow.Selected = true;
                            auditGrid.CurrentCell = gridRow.Cells[0];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (showError)
                {
                    MessageBox.Show(
                        "Das Audit Log konnte nicht geladen werden:\n" +
                        ex.Message,
                        "Audit Log",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            finally
            {
                if (!IsDisposed)
                {
                    reloadButton.Enabled = true;
                }

                wirdGeladen = false;
            }
        }

        private void auditGrid_SelectionChanged(
            object? sender,
            EventArgs e)
        {
            if (auditGrid.CurrentRow?.DataBoundItem is not AuditLogRow row)
            {
                detailsTextBox.Clear();
                return;
            }

            detailsTextBox.Text = JsonSchoenFormatieren(row.Changes);
        }

        private static string JsonSchoenFormatieren(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return "Keine Detailänderungen gespeichert.";
            }

            try
            {
                using JsonDocument document =
                    JsonDocument.Parse(json);

                return JsonSerializer.Serialize(
                    document.RootElement,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }
                );
            }
            catch
            {
                return json;
            }
        }

        private sealed class AuditLogRow
        {
            public int Id { get; set; }

            public DateTime Zeit { get; set; }

            public string Benutzer { get; set; } =
                string.Empty;

            public string? Rolle { get; set; }

            public string Aktion { get; set; } =
                string.Empty;

            public string Entitaet { get; set; } =
                string.Empty;

            public string? EntityId { get; set; }

            public string Quelle { get; set; } =
                string.Empty;

            public string? Changes { get; set; }
        }

        public void backBtn_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
