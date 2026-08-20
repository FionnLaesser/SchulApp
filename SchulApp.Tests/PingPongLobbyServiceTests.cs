extern alias RestApi;

using Xunit;
using LobbyService = RestApi::schulAppREST.Services.PingPongLobbyService;
using JoinStatus = RestApi::schulAppREST.Services.PingPongLobbyJoinStatus;
using CloseStatus = RestApi::schulAppREST.Services.PingPongLobbyCloseStatus;
using LeaveStatus = RestApi::schulAppREST.Services.PingPongLobbyLeaveStatus;

namespace SchulApp.Tests
{
    public sealed class PingPongLobbyServiceTests
    {
        [Fact]
        public void CreateLobby_CreatesSixCharacterCode()
        {
            LobbyService service = new LobbyService();

            var lobby = service.CreateLobby(1, "Host");

            Assert.Equal(6, lobby.Code.Length);
            Assert.Equal(1, lobby.HostUserId);
            Assert.Equal("Host", lobby.HostUsername);
            Assert.Equal("Waiting", lobby.Status);
        }

        [Fact]
        public void CreateLobby_UsesOnlyShareFriendlyCharacters()
        {
            const string allowed = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            LobbyService service = new LobbyService();

            var lobby = service.CreateLobby(1, "Host");

            Assert.All(lobby.Code, character => Assert.Contains(character, allowed));
            Assert.DoesNotContain('0', lobby.Code);
            Assert.DoesNotContain('1', lobby.Code);
            Assert.DoesNotContain('I', lobby.Code);
            Assert.DoesNotContain('O', lobby.Code);
        }

        [Fact]
        public void CreateLobby_ForDifferentHosts_CreatesDifferentCodes()
        {
            LobbyService service = new LobbyService();

            var firstLobby = service.CreateLobby(1, "Host1");
            var secondLobby = service.CreateLobby(2, "Host2");

            Assert.NotEqual(firstLobby.Code, secondLobby.Code);
        }

        [Fact]
        public void CreateLobby_ForManyHosts_CreatesUniqueCodes()
        {
            LobbyService service = new LobbyService();
            HashSet<string> codes = new(StringComparer.OrdinalIgnoreCase);

            for (int userId = 1; userId <= 100; userId++)
            {
                var lobby = service.CreateLobby(userId, $"Host{userId}");
                Assert.True(codes.Add(lobby.Code));
            }

            Assert.Equal(100, codes.Count);
        }

        [Fact]
        public void CreateLobby_ForSameHost_ReturnsExistingLobby()
        {
            LobbyService service = new LobbyService();

            var firstLobby = service.CreateLobby(1, "Host");
            var secondLobby = service.CreateLobby(1, "Host");

            Assert.Equal(firstLobby.Code, secondLobby.Code);
        }

        [Fact]
        public void JoinLobby_WithValidCode_AddsGuestAndMarksLobbyReady()
        {
            LobbyService service = new LobbyService();
            var lobby = service.CreateLobby(1, "Host");

            var result = service.JoinLobby(lobby.Code, 2, "Guest");

            Assert.Equal(JoinStatus.Success, result.Status);
            Assert.NotNull(result.Lobby);
            Assert.Equal(2, result.Lobby!.GuestUserId);
            Assert.Equal("Guest", result.Lobby.GuestUsername);
            Assert.Equal("Ready", result.Lobby.Status);
        }

        [Fact]
        public void JoinLobby_NormalizesWhitespaceAndLowercaseCode()
        {
            LobbyService service = new LobbyService();
            var lobby = service.CreateLobby(1, "Host");

            var result = service.JoinLobby(
                "  " + lobby.Code.ToLowerInvariant() + "  ",
                2,
                "Guest"
            );

            Assert.Equal(JoinStatus.Success, result.Status);
            Assert.Equal(2, result.Lobby!.GuestUserId);
        }

        [Fact]
        public void JoinLobby_SameGuestCanReconnectWithoutFillingLobbyAgain()
        {
            LobbyService service = new LobbyService();
            var lobby = service.CreateLobby(1, "Host");
            service.JoinLobby(lobby.Code, 2, "Guest");

            var result = service.JoinLobby(lobby.Code, 2, "Guest");

            Assert.Equal(JoinStatus.Success, result.Status);
            Assert.Equal(2, result.Lobby!.GuestUserId);
        }

        [Fact]
        public void JoinLobby_HostCannotJoinOwnLobby()
        {
            LobbyService service = new LobbyService();
            var lobby = service.CreateLobby(1, "Host");

            var result = service.JoinLobby(lobby.Code, 1, "Host");

            Assert.Equal(JoinStatus.SamePlayer, result.Status);
        }

        [Fact]
        public void JoinLobby_FullLobbyRejectsThirdPlayer()
        {
            LobbyService service = new LobbyService();
            var lobby = service.CreateLobby(1, "Host");
            service.JoinLobby(lobby.Code, 2, "Guest");

            var result = service.JoinLobby(lobby.Code, 3, "Third");

            Assert.Equal(JoinStatus.Full, result.Status);
            Assert.Equal(2, result.Lobby!.GuestUserId);
        }

        [Fact]
        public void JoinLobby_InvalidCodeReturnsNotFound()
        {
            LobbyService service = new LobbyService();

            var result = service.JoinLobby("ABC123", 2, "Guest");

            Assert.Equal(JoinStatus.NotFound, result.Status);
        }

        [Fact]
        public void JoinLobby_ClosedCodeReturnsNotFound()
        {
            LobbyService service = new LobbyService();
            var lobby = service.CreateLobby(1, "Host");
            service.CloseLobby(lobby.Code, 1);

            var result = service.JoinLobby(lobby.Code, 2, "Guest");

            Assert.Equal(JoinStatus.NotFound, result.Status);
        }

        [Fact]
        public void LeaveLobby_GuestLeavesLobbyAndReturnsToWaiting()
        {
            LobbyService service = new LobbyService();
            var lobby = service.CreateLobby(1, "Host");
            service.JoinLobby(lobby.Code, 2, "Guest");

            var leaveResult = service.LeaveLobby(lobby.Code, 2);
            var updatedLobby = service.GetLobby(lobby.Code);

            Assert.Equal(LeaveStatus.Success, leaveResult);
            Assert.NotNull(updatedLobby);
            Assert.Null(updatedLobby!.GuestUserId);
            Assert.Null(updatedLobby.GuestUsername);
            Assert.Equal("Waiting", updatedLobby.Status);
        }

        [Fact]
        public void LeaveLobby_UnrelatedPlayerIsRejected()
        {
            LobbyService service = new LobbyService();
            var lobby = service.CreateLobby(1, "Host");
            service.JoinLobby(lobby.Code, 2, "Guest");

            var result = service.LeaveLobby(lobby.Code, 99);

            Assert.Equal(LeaveStatus.NotInLobby, result);
            Assert.Equal(2, service.GetLobby(lobby.Code)!.GuestUserId);
        }

        [Fact]
        public void LeaveLobby_HostClosesLobbyForEveryone()
        {
            LobbyService service = new LobbyService();
            var lobby = service.CreateLobby(1, "Host");
            service.JoinLobby(lobby.Code, 2, "Guest");

            var result = service.LeaveLobby(lobby.Code, 1);

            Assert.Equal(LeaveStatus.LobbyClosed, result);
            Assert.Null(service.GetLobby(lobby.Code));
        }

        [Fact]
        public void CloseLobby_HostRemovesLobby()
        {
            LobbyService service = new LobbyService();
            var lobby = service.CreateLobby(1, "Host");

            var closeResult = service.CloseLobby(lobby.Code, 1);

            Assert.Equal(CloseStatus.Success, closeResult);
            Assert.Null(service.GetLobby(lobby.Code));
        }

        [Fact]
        public void CloseLobby_NonHostCannotCloseLobby()
        {
            LobbyService service = new LobbyService();
            var lobby = service.CreateLobby(1, "Host");

            var closeResult = service.CloseLobby(lobby.Code, 2);

            Assert.Equal(CloseStatus.NotHost, closeResult);
            Assert.NotNull(service.GetLobby(lobby.Code));
        }

        [Fact]
        public void CloseLobby_MissingLobbyReturnsNotFound()
        {
            LobbyService service = new LobbyService();

            var closeResult = service.CloseLobby("ABC123", 1);

            Assert.Equal(CloseStatus.NotFound, closeResult);
        }
    }
}
