# PingPong Multiplayer Manual Tests

This checklist covers interactive scenarios that cannot be tested reliably with unit tests alone.

## Prerequisites

- Build the current `master` candidate successfully.
- Start the SchulApp REST API and SchulApp normally.
- Have two different SchulApp user accounts available.
- For LAN tests, both PCs must be in the same local network.
- The host PC must allow TCP port `63636` through the Windows Firewall for the private network.

## Same-PC multiplayer

### MP-01 Create and join a local lobby

1. Start SchulApp client A and sign in as User A.
2. Start SchulApp client B and sign in as User B.
3. In client A, open PingPong Multiplayer and create a game.
4. Copy the displayed game code.
5. In client B, choose Join Game.
6. Enter `localhost` as Host / IP and paste the game code.

Expected:

- Both clients show the same game code.
- Player 1 is User A and Player 2 is User B.
- Lobby status changes to `Ready` on both clients.
- Both clients can open the multiplayer game.

### MP-02 Controls without changing window focus

1. Open both multiplayer game windows side by side.
2. Click only the Player 2 window.
3. Press `W` and `S`.
4. Press the up and down arrow keys.
5. Repeat with the Player 1 window focused.

Expected:

- Player 1 moves only with `W` / `S`.
- Player 2 moves only with the arrow keys.
- Both paddles remain synchronized in both windows.
- Clicking between the two game windows is not required for paddle movement.

### MP-03 Ball and score synchronization

1. Start a match with both clients visible.
2. Let the ball hit both paddles several times.
3. Allow Player 1 to score.
4. Allow Player 2 to score.

Expected:

- Both clients show the same `ball.png` position.
- Paddle collisions look consistent on both clients.
- The score changes at the same time on both clients.
- A point resets the ball consistently on both clients.

### MP-04 Game start and game end

1. Start a new multiplayer match.
2. Play until one player reaches 5 points.

Expected:

- Both clients start the same match.
- The match ends at exactly 5 points.
- Both clients show the same winner and final score.
- The authoritative host stores the final result only once.

### MP-05 Pause from Player 1

1. Start a match.
2. On Player 1, press `Q` or click Pause.
3. Note the ball position, both paddle positions, and score.
4. Wait several seconds.
5. Click `Fortsetzen`.

Expected:

- The pause menu appears on both clients.
- Ball movement stops on both clients.
- Paddle movement stops on both clients.
- Ball, paddle, and score values do not reset.
- Continuing resumes the same state.

### MP-06 Pause request from Player 2

1. Start a match.
2. On Player 2, press `Q` or click Pause.
3. Resume from Player 2.

Expected:

- Player 2 can request pause and resume.
- Player 1 confirms and broadcasts the synchronized pause state.
- Both clients enter and leave pause together.

### MP-07 Leave while paused

1. Pause a running match.
2. Click `Spiel verlassen` on one client.

Expected:

- The local multiplayer game closes cleanly.
- The remaining client stops the match when the connection is lost.
- SchulApp stays usable and can return to the lobby/menu.

## Lobby and validation failures

### MP-08 Invalid game code

1. Choose Join Game.
2. Enter a valid host but a game code that does not exist.

Expected:

- Join is rejected with a clear message.
- No game window opens.
- The multiplayer menu remains usable.

### MP-09 Closed lobby code

1. Create a lobby and copy its code.
2. Close the lobby as the host.
3. Try to join with the old code.

Expected:

- The old code is rejected.
- No stale lobby is opened.

### MP-10 Full lobby

1. Create a lobby with User A.
2. Join with User B.
3. Try to join the same code with User C.

Expected:

- User C is rejected because the lobby is full.
- User A and User B remain connected to their existing lobby.

### MP-11 Guest leaves before match start

1. Create a lobby with User A.
2. Join with User B.
3. User B clicks `Lobby verlassen` before opening the game.

Expected:

- User A returns to `Waiting` state.
- A new second player can join the same lobby.

### MP-12 Host closes lobby before match start

1. Create a lobby with User A.
2. Join with User B.
3. User A clicks `Lobby schliessen`.

Expected:

- The lobby becomes unavailable.
- User B receives a closed/not-found state on refresh.
- User B can return to the multiplayer menu.

## Connection failures

### MP-13 Unreachable host

1. Choose Join Game.
2. Enter an unused LAN IP address.
3. Enter any six-character code.

Expected:

- The request stops within the configured connection timeout.
- A useful host/IP/port/firewall message is shown.
- SchulApp does not freeze or crash.

### MP-14 REST/WebSocket host shutdown during match

1. Start a multiplayer match.
2. Stop the host REST process while the match is running.

Expected:

- The client detects the lost connection.
- Ball and paddle movement stop.
- The UI shows that the multiplayer connection was lost.
- The Back action remains usable.

### MP-15 Player disconnect during match

1. Start a multiplayer match.
2. Close one game window or terminate one client.

Expected:

- The remaining game stops instead of continuing with stale state.
- SchulApp remains responsive.
- Returning to the lobby/menu is possible.

## LAN multiplayer

### MP-16 Join from a second PC

1. On PC A, start the REST API and create a PingPong lobby.
2. Read the LAN IPv4 address shown in the lobby.
3. On PC B, sign in with another user.
4. Enter PC A's IPv4 address as Host / IP and enter the game code.

Expected:

- PC B joins the lobby through port `63636`.
- Both clients show `Ready`.
- The game opens on both PCs.

### MP-17 Full LAN match

1. Complete MP-16.
2. Play a full match to 5 points.
3. Pause and resume once during the match.

Expected:

- Paddle movement is synchronized across the LAN.
- Ball position and score are synchronized across the LAN.
- Pause state is synchronized across the LAN.
- Both PCs show the same final winner and score.

### MP-18 Firewall failure

1. Block inbound port `63636` on the host PC or use a host where the port is not reachable.
2. Attempt to join from PC B.

Expected:

- Join fails without crashing either app.
- The error message mentions host/IP, port `63636`, network, or Windows Firewall.

## Regression

### MP-19 Normal same-PC PingPong

1. Open the normal `Same PC` PingPong mode.
2. Select an opponent.
3. Start a match.
4. Move both paddles.
5. Pause and resume the game.
6. Finish the match.

Expected:

- Existing normal PingPong controls still work.
- Its pause menu still works.
- Ball image, score, game end, and result saving still work.
- Multiplayer changes do not alter normal PingPong behavior.

## Result recording

For every failed scenario, record:

- Test ID
- Date and time
- PC/client used
- Host/IP used where relevant
- Exact error message
- Screenshot if the problem is visual
- Relevant REST or SchulApp console output
