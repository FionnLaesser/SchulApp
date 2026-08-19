using SchulApp.Models;
using SchulApp.Services;
using Xunit;

namespace SchulApp.Tests
{
    public class TimetableDataTransferServiceTests
    {
        [Fact]
        public void ValidateCsv_AcceptsExportCompatibleTimetableRow()
        {
            string csv =
                "ID;Kurs-ID;Wochentag;Start;Ende;Kurs;Klasse;Raum\n" +
                "1;12;Montag;08:00;08:45;Mathematik;AP24a;101\n";

            TimetableCsvValidationResult result =
                TimetableDataTransferService.ValidateCsv(
                    csv,
                    new[] { 12 }
                );

            Assert.Empty(result.Errors);

            StundenplanModel entry = Assert.Single(result.Entries);
            Assert.Equal(12, entry.KursId);
            Assert.Equal((byte)1, entry.Wochentag);
            Assert.Equal(new TimeSpan(8, 0, 0), entry.Startzeit);
            Assert.Equal(new TimeSpan(8, 45, 0), entry.Endzeit);
            Assert.Equal("101", entry.Raum);
        }

        [Fact]
        public void ValidateCsv_RejectsUnknownCourseAndInvalidTimeRange()
        {
            string csv =
                "Kurs-ID;Wochentag;Start;Ende;Raum\n" +
                "99;Montag;08:00;08:45;101\n" +
                "12;Dienstag;10:00;09:00;102\n";

            TimetableCsvValidationResult result =
                TimetableDataTransferService.ValidateCsv(
                    csv,
                    new[] { 12 }
                );

            Assert.Empty(result.Entries);
            Assert.Equal(2, result.Errors.Count);
            Assert.Contains(
                result.Errors,
                error => error.Contains("existiert nicht")
            );
            Assert.Contains(
                result.Errors,
                error => error.Contains("Endzeit")
            );
        }

        [Fact]
        public void ValidateCsv_RejectsExistingDuplicate()
        {
            string csv =
                "KursId;Wochentag;Start;Ende;Raum\n" +
                "4;3;13:15;14:00;204\n";

            StundenplanModel existing = new StundenplanModel
            {
                KursId = 4,
                Wochentag = 3,
                Startzeit = new TimeSpan(13, 15, 0),
                Endzeit = new TimeSpan(14, 0, 0),
                Raum = "204"
            };

            TimetableCsvValidationResult result =
                TimetableDataTransferService.ValidateCsv(
                    csv,
                    new[] { 4 },
                    new[] { existing }
                );

            Assert.Empty(result.Entries);
            Assert.Single(result.Errors);
            Assert.Contains("bereits vorhanden", result.Errors[0]);
        }

        [Fact]
        public void ValidateCsv_AcceptsWeekdayNumberAndOptionalRoom()
        {
            string csv =
                "Kurs-ID;Wochentag;Start;Ende\n" +
                "5;5;09:00;09:45\n";

            TimetableCsvValidationResult result =
                TimetableDataTransferService.ValidateCsv(
                    csv,
                    new[] { 5 }
                );

            Assert.Empty(result.Errors);

            StundenplanModel entry = Assert.Single(result.Entries);
            Assert.Equal((byte)5, entry.Wochentag);
            Assert.Null(entry.Raum);
        }
    }
}
