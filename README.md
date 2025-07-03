# Connect Four Blazor App

The Connect Four codebase is a Blazor web application that implements the classic Connect Four game. Here’s an overview of its structure and main components:

---

## Main Components

### [`GameState`](GameState.cs)
- **Purpose:** Central class for managing the game’s logic and state.
- **Responsibilities:**
  - Tracks the board as a list of 42 integers (6 rows × 7 columns).
  - Calculates all possible winning positions (horizontal, vertical, diagonal) at startup.
  - Provides methods to play a piece, check for a win/tie, and reset the board.
  - Maintains player colors and tallies for wins and ties.

### [`Board.razor`](Components/Board.razor)
- **Purpose:** The main UI component for the Connect Four board.
- **Features:**
  - Renders the game board and interactive column selectors.
  - Animates piece drops using CSS classes.
  - Displays the current turn, win/tie messages, and a running tally of wins/ties.
  - Accepts parameters for board and player colors, which are passed to CSS variables for dynamic styling.
  - Handles game logic by calling methods on the shared `GameState` service.

### [`Pages/Home.razor`](Components/Pages/Home.razor)
- **Purpose:** The home page that hosts the `Board` component.
- **Features:** Sets initial colors for the board and players via component parameters.

---

## Supporting Files

- **CSS:** [`wwwroot/app.css`](wwwroot/app.css) and component-specific CSS files style the UI.
- **Layout:** [`MainLayout.razor`](Components/Layout/MainLayout.razor) and [`NavMenu.razor`](Components/Layout/NavMenu.razor) provide navigation and page structure.
- **Program Setup:** [`Program.cs`](Program.cs) configures services and sets up the Blazor app, registering `GameState` as a singleton for shared state.

---

## Game Flow

1. **Initialization:**  
   The board and player colors are set, and the board is reset.
2. **Gameplay:**  
   Players click on columns to drop pieces. The board updates, and the game state checks for wins or ties.
3. **End of Game:**  
   Win/tie messages and tallies are updated. Players can reset the game to play again.

---

## Customization

- **Colors:**  
  The board and player colors can be customized via parameters, and these are reflected in both the UI and game state.
- **Tally:**  
  The app keeps a running tally of wins and ties, displayed with line breaks for readability.

---

**Summary:**  
This codebase provides a complete, interactive Connect Four game using Blazor, with a clear separation between UI and game logic, support for customization,