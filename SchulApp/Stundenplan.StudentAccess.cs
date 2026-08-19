using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;
using SchulApp.Services;

namespace SchulApp
{
    public partial class Stundenplan
    {
        private bool timetableAccessInitialized;
        private bool studentTimetableRestricted;
        private int? studentVisibleClassId;
        private string? timetableAccessError;
        private bool timetableAccessErrorShown;

        protected override void OnShown(EventArgs e)
        {
            EnsureTimetableAccessInitialized();

            if (studentTimetableRestricted)
            {
                ApplyStudentTimetableRestriction();
            }

            base.OnShown(e);

            if (
                !timetableAccessErrorShown &&
                !string.IsNullOrWhiteSpace(timetableAccessError))
            {
                timetableAccessErrorShown = true;

                MessageBox.Show(
                    this,
                    timetableAccessError,
                    "Stundenplan-Zugriff",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void EnsureTimetableAccessInitialized()
        {
            if (timetableAccessInitialized)
            {
                return;
            }

            timetableAccessInitialized = true;

            if (BenutzerSession.BenutzerId <= 0)
            {
                if (BenutzerSession.Rolle == LoginBenutzer.RolleSchueler)
                {
                    RestrictToEmptyStudentTimetable(
                        "Der angemeldete Benutzer konnte nicht bestimmt werden."
                    );
                }

                return;
            }

            try
            {
                studentVisibleClassId =
                    TimetableAccessService.GetVisibleClassId(
                        BenutzerSession.BenutzerId
                    );

                studentTimetableRestricted =
                    studentVisibleClassId.HasValue;
            }
            catch (Exception ex)
            {
                RestrictToEmptyStudentTimetable(ex.Message);
            }

            if (studentTimetableRestricted)
            {
                reloadBtn.Click += RestrictedStudentReload_Click;
            }
        }

        private void RestrictToEmptyStudentTimetable(string message)
        {
            studentTimetableRestricted = true;
            studentVisibleClassId = -1;
            timetableAccessError =
                message +
                Environment.NewLine +
                "Es werden keine Stundenplandaten angezeigt.";
        }

        private void RestrictedStudentReload_Click(object? sender, EventArgs e)
        {
            ApplyStudentTimetableRestriction();
        }

        private void ApplyStudentTimetableRestriction()
        {
            if (!studentTimetableRestricted || IsDisposed)
            {
                return;
            }

            int klasseId = studentVisibleClassId ?? -1;

            try
            {
                using SchulAppContext context = new SchulAppContext();

                var kurse = context.Kurse
                    .AsNoTracking()
                    .Where(k => k.KlasseId == klasseId)
                    .OrderBy(k => k.Name)
                    .ThenBy(k => k.Klasse.Bezeichnung)
                    .Select(k => new TimetableCourseSelectionItem
                    {
                        KursId = k.KursId,
                        Anzeige =
                            k.Name +
                            " | " +
                            k.Klasse.Bezeichnung +
                            " | " +
                            k.Lehrer.Name
                    })
                    .ToList();

                newKurs.DisplayMember = nameof(TimetableCourseSelectionItem.Anzeige);
                newKurs.ValueMember = nameof(TimetableCourseSelectionItem.KursId);
                newKurs.DataSource = kurse;

                editKurs.DisplayMember = nameof(TimetableCourseSelectionItem.Anzeige);
                editKurs.ValueMember = nameof(TimetableCourseSelectionItem.KursId);
                editKurs.DataSource = kurse.ToList();

                var plan = context.Stundenplan
                    .AsNoTracking()
                    .Where(s => s.Kurs.KlasseId == klasseId)
                    .OrderBy(s => s.Wochentag)
                    .ThenBy(s => s.Startzeit)
                    .Select(s => new
                    {
                        s.StundenplanId,
                        s.KursId,
                        WochentagNr = s.Wochentag,
                        Wochentag =
                            s.Wochentag == 1 ? "Montag" :
                            s.Wochentag == 2 ? "Dienstag" :
                            s.Wochentag == 3 ? "Mittwoch" :
                            s.Wochentag == 4 ? "Donnerstag" :
                            s.Wochentag == 5 ? "Freitag" :
                            "",
                        s.Startzeit,
                        s.Endzeit,
                        Kurs = s.Kurs.Name,
                        Klasse = s.Kurs.Klasse.Bezeichnung,
                        Lehrer = s.Kurs.Lehrer.Name,
                        s.Raum
                    })
                    .ToList();

                stundenplanGrid.DataSource = plan;
                FormatRestrictedTimetableGrid();
                stundenplanGrid.ClearSelection();
                AuswahlZuruecksetzen();
            }
            catch (Exception ex)
            {
                stundenplanGrid.DataSource = null;
                newKurs.DataSource = null;
                editKurs.DataSource = null;

                if (!timetableAccessErrorShown)
                {
                    timetableAccessError =
                        "Dein Stundenplan konnte nicht geladen werden." +
                        Environment.NewLine +
                        ex.Message;
                }
            }
        }

        private void FormatRestrictedTimetableGrid()
        {
            if (stundenplanGrid.Columns["StundenplanId"] is DataGridViewColumn id)
            {
                id.HeaderText = "ID";
                id.Width = 45;
            }

            if (stundenplanGrid.Columns["KursId"] is DataGridViewColumn kursId)
            {
                kursId.Visible = false;
            }

            if (stundenplanGrid.Columns["WochentagNr"] is DataGridViewColumn tagNr)
            {
                tagNr.Visible = false;
            }

            if (stundenplanGrid.Columns["Wochentag"] is DataGridViewColumn tag)
            {
                tag.Width = 90;
            }

            if (stundenplanGrid.Columns["Startzeit"] is DataGridViewColumn start)
            {
                start.HeaderText = "Von";
                start.DefaultCellStyle.Format = @"hh\:mm";
                start.Width = 60;
            }

            if (stundenplanGrid.Columns["Endzeit"] is DataGridViewColumn ende)
            {
                ende.HeaderText = "Bis";
                ende.DefaultCellStyle.Format = @"hh\:mm";
                ende.Width = 60;
            }

            if (stundenplanGrid.Columns["Kurs"] is DataGridViewColumn kurs)
            {
                kurs.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            if (stundenplanGrid.Columns["Klasse"] is DataGridViewColumn klasse)
            {
                klasse.Width = 75;
            }

            if (stundenplanGrid.Columns["Lehrer"] is DataGridViewColumn lehrer)
            {
                lehrer.Width = 140;
            }

            if (stundenplanGrid.Columns["Raum"] is DataGridViewColumn raum)
            {
                raum.Width = 60;
            }
        }

        private sealed class TimetableCourseSelectionItem
        {
            public int KursId { get; set; }

            public string Anzeige { get; set; } = string.Empty;
        }
    }
}
